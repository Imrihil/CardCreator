using CardCreator.Features.Drawing.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CardCreator.Features.Drawing.Text.Model
{
    public sealed class Article : IDisposable, IDrawable
    {
        public Font Font { get; }
        public StringFormatExtended StringFormat { get; }
        public RectangleF LayoutRectangle { get; }
        public SizeF Size { get; }
        private List<Paragraph> Paragraphs { get; }
        private float LineSeparatorHeight { get; }

        private bool disposed = false;

        public Article(IIconProvider iconProvider, Graphics graphics, string content, StringFormatExtended stringFormat, FontFamily fontFamily, int maxSize, int minSize, Color color, Color shadowColor, int shadowSize, bool wrapLines, int shortestAloneWords, RectangleF layoutRectangle)
        {
            StringFormat = stringFormat;
            LayoutRectangle = layoutRectangle;

            Paragraphs = GetParagraphs(iconProvider, graphics, content, fontFamily, maxSize, minSize, color, shadowColor, shadowSize, wrapLines, shortestAloneWords, out var font, out var lineSeparatorHeight);
            Font = font;
            LineSeparatorHeight = lineSeparatorHeight;
            Size = GetSize();
        }

        public void Draw(Graphics graphics)
        {
            foreach (var paragraph in Paragraphs)
                paragraph.Draw(graphics);
        }

        public override string ToString() =>
            string.Join(Environment.NewLine, Paragraphs);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                foreach (var paragraph in Paragraphs)
                    paragraph.Dispose();
                Font?.Dispose();
            }

            disposed = true;
        }

        private List<Paragraph> GetParagraphs(IIconProvider iconProvider, Graphics graphics, string content, FontFamily fontFamily, int maxSize, int minSize, Color color, Color shadowColor, int shadowSize, bool wrapLines, int shortestAloneWords, out Font font, out float lineSeparatorHeight)
        {
            List<Paragraph> paragraphs = null;
            var paragraphContents = content.Split(new[] { "\n", "\r\n" }, StringSplitOptions.None);
            var size = new SizeF();
            font = null;

            for (var emSize = maxSize; emSize >= minSize; --emSize)
            {
                font?.Dispose();
                paragraphs?.ForEach(paragraph => paragraph.Dispose());

                font = new Font(fontFamily, emSize, GraphicsUnit.Pixel);
                paragraphs = new List<Paragraph>();
                size = new SizeF();

                foreach (var paragraphContent in paragraphContents)
                {
                    var paragraph = new Paragraph(iconProvider, graphics, paragraphContent, StringFormat, font, color, shadowColor, shadowSize, wrapLines, shortestAloneWords,
                        new RectangleF(LayoutRectangle.X, LayoutRectangle.Y + size.Height, LayoutRectangle.Width, LayoutRectangle.Height - size.Height));
                    paragraphs.Add(paragraph);
                    size.Width = Math.Max(size.Width, paragraph.Size.Width);
                    size.Height += paragraph.Size.Height;
                }

                if (size.Width <= LayoutRectangle.Width &&
                    size.Height <= LayoutRectangle.Height)
                {
                    break;
                }
            }

            var shift = GetFirstShift(StringFormat.LineAlignment, size.Height);
            var linesCount = paragraphs.Aggregate(0, (count, paragraph) => count += paragraph.LinesCount);
            lineSeparatorHeight = linesCount < 2 || StringFormat.LineAlignment != StringAlignmentExtended.Justify ?
                0 :
                (LayoutRectangle.Height - size.Height) / (linesCount - 1);

            var i = 0;
            foreach (var paragraph in paragraphs)
            {
                paragraph.Shift(shift + i * lineSeparatorHeight, lineSeparatorHeight);
                i += paragraph.LinesCount;
            }

            return paragraphs;
        }

        private float GetFirstShift(StringAlignmentExtended alignment, float contentHeight) =>
            alignment switch
            {
                StringAlignmentExtended.Center => (int)Math.Floor((LayoutRectangle.Height - contentHeight) / 2),
                StringAlignmentExtended.Far => LayoutRectangle.Height - contentHeight,
                _ => 0
            };

        private SizeF GetSize()
        {
            var size = Paragraphs.Aggregate(new SizeF(), (size, paragraph) =>
            {
                size.Width = Math.Max(size.Width, paragraph.Size.Width);
                size.Height += paragraph.Size.Height;
                return size;
            });

            if (Paragraphs.Any())
                size.Height += (Paragraphs.Count - 1) * LineSeparatorHeight;

            return size;
        }
    }
}

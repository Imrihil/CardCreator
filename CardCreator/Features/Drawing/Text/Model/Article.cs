using CardCreator.Features.Drawing.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CardCreator.Features.Drawing.Text.Model
{
    public sealed class Article : IDisposable, IDrawable
    {
        private List<Paragraph> Paragraphs { get; }
        public StringFormatExtended StringFormat { get; }
        public RectangleF LayoutRectangle { get; }
        public SizeF Size { get; }

        private bool disposed = false;

        public Article(IIconProvider iconProvider, Graphics graphics, string content, StringFormatExtended stringFormat, FontFamily fontFamily, int maxSize, int minSize, Color color, Color shadowColor, int shadowSize, bool wrapLines, int shortestAloneWords, RectangleF layoutRectangle)
        {
            StringFormat = stringFormat;
            LayoutRectangle = layoutRectangle;
            Paragraphs = GetParagraphs(iconProvider, graphics, content, fontFamily, maxSize, minSize, color, shadowColor, shadowSize, wrapLines, shortestAloneWords);
        }

        private List<Paragraph> GetParagraphs(IIconProvider iconProvider, Graphics graphics, string content, FontFamily fontFamily, int maxSize, int minSize, Color color, Color shadowColor, int shadowSize, bool wrapLines, int shortestAloneWords)
        {
            var font = GetAdjustedFont(graphics, fontFamily, maxSize, minSize, wrapLines, LayoutRectangle, out var linesCount);
            var interline = StringFormat.LineAlignment == StringAlignmentExtended.Justify ?
                (LayoutRectangle.Height - font.Height) / ((linesCount - 1) * font.Height) :
                1;

            return content.Split(new[] { "\n", "\r\n" }, StringSplitOptions.None)
                .Select(paragraphContent => new Paragraph(iconProvider, graphics, paragraphContent, StringFormat, interline, font, color, shadowColor, shadowSize, wrapLines, shortestAloneWords, LayoutRectangle))
                .ToList();
        }

        private Font GetAdjustedFont(Graphics graphics, FontFamily fontFamily, int maxSize, int minSize, bool wrapLines, RectangleF layoutRectangle, out int linesCount)
        {
            linesCount = 0;
            for (var emSize = maxSize; emSize >= minSize; --emSize)
            {
                var font = new Font(fontFamily, emSize, GraphicsUnit.Pixel);

                var size = new SizeF();
                foreach (var paragraph in this)
                {
                    var paragraphSize = paragraph.Measure(graphics, font, layoutRectangle.Width, wrapLines, out var paragraphLinesCount);
                    size.Width = Math.Max(size.Width, paragraphSize.Width);
                    size.Height += paragraphSize.Height;
                    linesCount += paragraphLinesCount;
                }

                if (size.Width <= layoutRectangle.Width &&
                    size.Height <= layoutRectangle.Height)
                    return font;

                linesCount = 0;
            }

            return new Font(fontFamily, minSize, GraphicsUnit.Pixel);
        }

        public void Draw(Graphics graphics)
        {
            var paragraphLayoutRectangle = new RectangleF(LayoutRectangle.X, LayoutRectangle.Y, LayoutRectangle.Width, LayoutRectangle.Height);
            foreach (var paragraph in this)
            {
                paragraph.Draw(graphics);
                paragraphLayoutRectangle = new RectangleF(paragraphLayoutRectangle.X, paragraphLayoutRectangle.Y + Size.Height, LayoutRectangle.Width, paragraphLayoutRectangle.Height - Size.Height);
            }
        }

        public override string ToString() =>
            string.Join(Environment.NewLine, this);

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
            }

            disposed = true;
        }
    }
}

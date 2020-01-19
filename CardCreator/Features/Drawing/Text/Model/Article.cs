using CardCreator.Features.Drawing.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CardCreator.Features.Drawing.Text.Model
{
    public sealed class Article : List<Paragraph>, IDisposable
    {
        private bool disposed = false;

        public Article(IIconProvider iconProvider, int shortestAloneWords, string content) : base(InitializeParagraphs(iconProvider, shortestAloneWords, content)) { }

        public void Draw(Graphics graphics, StringFormatExtended stringFormat, FontFamily fontFamily, int maxSize, int minSize, Color color, Color shadowColor, int shadowSize, bool wrapLines, RectangleF layoutRectangle)
        {
            var font = GetAdjustedFont(graphics, fontFamily, maxSize, minSize, wrapLines, layoutRectangle, out var linesCount);
            var interline = stringFormat.LineAlignment == StringAlignmentExtended.Justify ? 
                (layoutRectangle.Height - font.Height) / ((linesCount - 1) * font.Height) :
                1;
            var paragraphLayoutRectangle = new RectangleF(layoutRectangle.X, layoutRectangle.Y, layoutRectangle.Width, layoutRectangle.Height);
            foreach (var paragraph in this)
            {
                paragraph.Draw(graphics, stringFormat, interline, font, color, shadowColor, shadowSize, wrapLines, paragraphLayoutRectangle, out var size);
                paragraphLayoutRectangle = new RectangleF(paragraphLayoutRectangle.X, paragraphLayoutRectangle.Y + size.Height, layoutRectangle.Width, paragraphLayoutRectangle.Height - size.Height);
            }
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
                foreach (var paragraph in this)
                    paragraph.Dispose();
            }

            disposed = true;
        }

        private static IEnumerable<Paragraph> InitializeParagraphs(IIconProvider iconProvider, int shortestAloneWords, string content) =>
            content.Split(new[] { "\n", "\r\n" }, StringSplitOptions.None).Select(paragraphContent => new Paragraph(iconProvider, shortestAloneWords, paragraphContent));
    }
}

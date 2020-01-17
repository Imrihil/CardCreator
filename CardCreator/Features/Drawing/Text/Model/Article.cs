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

        public Article(IIconProvider iconProvider, string content) : base(InitializeParagraphs(iconProvider, content)) { }

        private static IEnumerable<Paragraph> InitializeParagraphs(IIconProvider iconProvider, string content) =>
            content.Split(new[] { "\n", "\r\n" }, StringSplitOptions.None).Select(paragraphContent => new Paragraph(iconProvider, paragraphContent));

        public void Draw(Graphics graphics, StringFormatExtended stringFormat, FontFamily fontFamily, int maxSize, int minSize, Color color, Color shadowColor, int shadowSize, bool wrapLines, RectangleF layoutRectangle)
        {
            graphics.DrawAdjustedStringWithShadow(ToString(), fontFamily, color, shadowColor, shadowSize, layoutRectangle, maxSize, stringFormat, minSize, true, wrapLines);
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
    }
}

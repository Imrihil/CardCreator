using CardCreator.Features.Drawing.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CardCreator.Features.Drawing.Text.Model
{
    public sealed class Paragraph : List<Word>, IDisposable
    {
        private const string Separator = " ";
        private bool disposed = false;
        private readonly int shortestAloneWords;

        public Paragraph(IIconProvider iconProvider, int shortestAloneWords, string content) : base(InitializeWords(iconProvider, content))
        {
            this.shortestAloneWords = shortestAloneWords;
        }

        public List<Line> GetLines(Graphics graphics, Font font, float width, out SizeF size)
        {
            size = new SizeF();
            SizeF lineSize;
            var lines = new List<Line>();
            var line = new Line();
            foreach (var word in this)
            {
                while (!line.TryAdd(word, width, graphics, font))
                {
                    var newLine = line.TrimAloneWords(shortestAloneWords);
                    lines.Add(line);
                    lineSize = line.Measure(graphics, font);
                    size.Width = Math.Max(size.Width, lineSize.Width);
                    size.Height += lineSize.Height;
                    line = newLine;
                }
            }
            lines.Add(line);
            lineSize = line.Measure(graphics, font);
            size.Width = Math.Max(size.Width, lineSize.Width);
            size.Height += lineSize.Height;

            return lines;
        }

        internal void Draw(Graphics graphics, StringFormatExtended stringFormat, float interline, Font font, Color color, Color shadowColor, int shadowSize, bool wrapLines, RectangleF layoutRectangle, out SizeF size)
        {
            if (!wrapLines)
            {
                var line = new Line(this);
                line.Draw(graphics, stringFormat, font, color, shadowColor, shadowSize, layoutRectangle);
                size = line.Measure(graphics, font);
            }
            else
            {
                var lines = GetLines(graphics, font, layoutRectangle.Width, out size);
                var lineLayoutRectangle = layoutRectangle;
                foreach (var line in lines)
                {
                    line.Draw(graphics, stringFormat, font, color, shadowColor, shadowSize, lineLayoutRectangle);
                    lineLayoutRectangle = new RectangleF(lineLayoutRectangle.X, lineLayoutRectangle.Y + interline * font.Height, layoutRectangle.Width, lineLayoutRectangle.Height - interline * font.Height);
                }
            }
        }

        public static SizeF GetSeparatorSize(Graphics graphics, Font font) =>
            graphics.MeasureString(Separator, font);

        public SizeF Measure(Graphics graphics, Font font, float width, bool wrapLines, out int linesCount)
        {
            if (!wrapLines)
            {
                linesCount = 1;
                return new Line(this).Measure(graphics, font);
            }

            var lines = GetLines(graphics, font, width, out var size);
            linesCount = lines.Count;
            return size;
        }

        public override string ToString() =>
            string.Join(' ', this);

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
                foreach (var word in this)
                    word.Dispose();
            }

            disposed = true;
        }

        private static IEnumerable<Word> InitializeWords(IIconProvider iconProvider, string content) =>
            content.Split(' ').Select(wordContent => new Word(iconProvider, wordContent));
    }
}

using CardCreator.Features.Drawing.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

namespace CardCreator.Features.Drawing.Text.Model
{
    public sealed class Paragraph : IDisposable, IDrawable
    {
        private const string Separator = " ";

        private List<Line> Lines { get; }
        public float Interline { get; }
        public bool WrapLines { get; }
        public RectangleF LayoutRectangle { get; }
        public SizeF Size { get; }

        private bool disposed = false;

        public Paragraph(IIconProvider iconProvider, Graphics graphics, string content, StringFormatExtended stringFormat, float interline, Font font, Color color, Color shadowColor, int shadowSize, bool wrapLines, int shortestAloneWords, RectangleF layoutRectangle)
        {
            Interline = interline;
            WrapLines = wrapLines;
            LayoutRectangle = layoutRectangle;
            Lines = GetLines(iconProvider, graphics, content, font, color, shadowColor, shadowSize, layoutRectangle.Width, wrapLines, shortestAloneWords);
        }

        private List<Line> GetLines(IIconProvider iconProvider, Graphics graphics, string content, Font font, Color color, Color shadowColor, int shadowSize, float width, bool wrapLines, int shortestAloneWords)
        {
            var lines = new List<Line>();
            if (!wrapLines)
            {
                lines.Add(new Line(this));
                return lines;
            }
            size = new SizeF();
            SizeF lineSize;
            var line = new Line();
            foreach (var word in content.Split(' ').Select(wordContent => new Word(iconProvider, graphics, wordContent, font, color, shadowColor, shadowSize)))
            {
                while (!line.TryAdd(word, width, graphics, font))
                {
                    var newLine = line.TrimAloneWords(ShortestAloneWords);
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

        public void Draw(Graphics graphics)
        {
            var lineLayoutRectangle = LayoutRectangle;
            foreach (var line in Lines)
            {
                line.Draw(graphics);
                lineLayoutRectangle = new RectangleF(lineLayoutRectangle.X, lineLayoutRectangle.Y + Interline * Font.Height, LayoutRectangle.Width, lineLayoutRectangle.Height - Interline * Font.Height);
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
                foreach (var line in Lines)
                    line.Dispose();
            }

            disposed = true;
        }
    }
}

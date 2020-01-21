using CardCreator.Features.Drawing.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

namespace CardCreator.Features.Drawing.Text.Model
{
    public sealed class Paragraph : List<Word>, IDisposable, IDrawable
    {
        private const string Separator = " ";

        public ReadOnlyCollection<Line> Lines { get; }
        public bool WrapLines { get; }
        public int ShortestAloneWords { get; }
        public RectangleF LayoutRectangle { get; }
        public float Interline { get; }

        private SizeF size;
        public SizeF Size
        {
            get
            {
                return size;
            }
        }

        private bool disposed = false;

        public Paragraph(IIconProvider iconProvider, string content, StringFormatExtended stringFormat, Font font, Color color, Color shadowColor, int shadowSize, bool wrapLines, int shortestAloneWords, RectangleF layoutRectangle) : base()
        {
            WrapLines = wrapLines;
            ShortestAloneWords = shortestAloneWords;
            LayoutRectangle = layoutRectangle;

            InitializeWords(iconProvider, content, font, color, shadowColor, shadowSize);

            using var image = new Bitmap((int)Math.Ceiling(layoutRectangle.Width), (int)Math.Ceiling(layoutRectangle.Height));
            using var graphics = Graphics.FromImage(image);
            Lines = GetLines(graphics, font, layoutRectangle.Width, wrapLines).AsReadOnly();
        }

        private static void InitializeWords(IIconProvider iconProvider, string content, Font font, Color color, Color shadowColor, int shadowSize)
        {
            foreach (var wordContent in content.Split(' '))
            {
                Add(new Word(iconProvider, wordContent, font, color, shadowColor, shadowSize));
            }
        }

        private List<Line> GetLines(Graphics graphics, Font font, float width, bool wrapLines)
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
            foreach (var word in this)
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
                foreach (var word in this)
                    word.Dispose();
            }

            disposed = true;
        }
    }
}

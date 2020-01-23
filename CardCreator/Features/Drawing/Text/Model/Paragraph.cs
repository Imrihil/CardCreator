using CardCreator.Features.Drawing.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CardCreator.Features.Drawing.Text.Model
{
    public sealed class Paragraph : IDisposable, IDrawable
    {
        public bool WrapLines { get; }
        public int LinesCount => Lines.Count;
        public RectangleF LayoutRectangle { get; private set; }
        public SizeF Size { get; }
        private List<Line> Lines { get; }
        private float LineSeparatorHeight { get; }
        private float SeparatorWidth { get; }

        private bool disposed = false;

        public Paragraph(IIconProvider iconProvider, Graphics graphics, string content, StringFormatExtended stringFormat,
            Font font, Color color, Color shadowColor, int shadowSize, bool wrapLines, int shortestAloneWords, RectangleF layoutRectangle)
        {
            WrapLines = wrapLines;
            LayoutRectangle = layoutRectangle;

            SeparatorWidth = Line.GetDefaultSeparatorWidth(graphics, font);
            Lines = GetLines(iconProvider, graphics, content, stringFormat, font, color, shadowColor, shadowSize, wrapLines, shortestAloneWords);
            Size = GetSize();
        }

        public void Draw(Graphics graphics)
        {
            foreach (var line in Lines)
                line.Draw(graphics);
        }

        public void Shift(float height, float lineSeparatorHeight)
        {
            LayoutRectangle = new RectangleF(
                LayoutRectangle.X, LayoutRectangle.Y + height,
                LayoutRectangle.Width, LayoutRectangle.Height);

            var i = 0;
            Lines.ForEach(line => line.Shift(height + lineSeparatorHeight * i++));
        }

        public override string ToString() =>
            string.Join(' ', Lines);

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

        private List<Line> GetLines(IIconProvider iconProvider, Graphics graphics, string content, StringFormatExtended stringFormat, Font font,
            Color color, Color shadowColor, int shadowSize, bool wrapLines, int shortestAloneWords)
        {
            if (!wrapLines)
                return new[] { new Line(iconProvider, graphics, content, stringFormat, font, color, shadowColor, shadowSize, shortestAloneWords, true, LayoutRectangle) }.ToList();

            var lines = new List<Line>();

            var words = content.Split(Line.Separator)
                .Select(wordContent =>
                new Word(iconProvider, graphics, wordContent, font, color, shadowColor, shadowSize, shortestAloneWords,
                new PointF(LayoutRectangle.X, LayoutRectangle.Y))).ToList();

            var startIdx = 0;
            while (startIdx < words.Count)
            {
                var endIdx = GetBreakLineIdx(words, startIdx, font);
                var line = new Line(graphics, words.GetRange(startIdx, endIdx - startIdx), stringFormat, font,
                    endIdx == words.Count,
                    new RectangleF(LayoutRectangle.X, LayoutRectangle.Y, LayoutRectangle.Width, font.Height));
                line.Shift(lines.Count * font.Height);
                lines.Add(line);
                startIdx = endIdx;
            }

            return lines;
        }

        private int GetBreakLineIdx(List<Word> words, int startIdx, Font font)
        {
            if (!words.Any())
                return 0;

            List<Word> wordsRange = null;
            var lIdx = startIdx + 1;
            var rIdx = words.Count;
            while (lIdx < rIdx)
            {
                var mIdx = (lIdx + rIdx) / 2;
                wordsRange = words.GetRange(startIdx, mIdx - startIdx + 1);
                var lineSize = Line.Measure(wordsRange, font, SeparatorWidth);
                if (lineSize.Width < LayoutRectangle.Width)
                    lIdx = mIdx + 1;
                else
                    rIdx = mIdx;
            }
            wordsRange = words.GetRange(startIdx, lIdx - startIdx);

            if (lIdx - startIdx <= 1 || wordsRange.All(word => word.IsAloneWord) || lIdx == words.Count)
                return lIdx;

            wordsRange.Reverse();
            foreach (var word in wordsRange)
            {
                if (word.IsAloneWord)
                    lIdx--;
                else
                    return lIdx;
            }

            return lIdx;
        }

        private SizeF GetSize()
        {
            var size = Lines.Aggregate(new SizeF(), (size, line) =>
            {
                size.Width = Math.Max(size.Width, line.Size.Width);
                size.Height += line.Size.Height;
                return size;
            });

            if (Lines.Any())
                size.Height += (Lines.Count - 1) * LineSeparatorHeight;

            return size;
        }
    }
}

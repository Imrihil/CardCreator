using CardCreator.Features.Drawing;
using CardCreator.Features.Drawing.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CardCreator.Features.Cards.Model
{
    public sealed class Line : IDisposable, IDrawable
    {
        public const string Separator = " ";

        public StringFormatExtended StringFormat { get; }
        public bool IsLast { get; }
        public RectangleF LayoutRectangle { get; private set; }
        public SizeF Size { get; }
        private List<Word> Words { get; }
        private float SeparatorWidth { get; }

        private bool disposed = false;

        public Line(Graphics graphics, List<Word> words, StringFormatExtended stringFormat, Font font, bool isLast, RectangleF layoutRectangle)
        {
            StringFormat = stringFormat;
            IsLast = isLast;
            LayoutRectangle = layoutRectangle;

            Words = words;
            SeparatorWidth = GetSeparatorWidth(graphics, font);
            Size = GetSize(font);
        }

        public Line(IIconProvider iconProvider, Graphics graphics, string content, StringFormatExtended stringFormat,
            Font font, Color color, Color shadowColor, int shadowSize, int shortestAloneWords, bool isLast, RectangleF layoutRectangle)
        {
            StringFormat = stringFormat;
            IsLast = isLast;
            LayoutRectangle = layoutRectangle;

            Words = GetWords(iconProvider, graphics, content, font, color, shadowColor, shadowSize, shortestAloneWords, layoutRectangle);
            SeparatorWidth = GetSeparatorWidth(graphics, font);
            Size = GetSize(font);
        }

        public void Draw(Graphics graphics)
        {
            foreach (var word in Words)
                word.Draw(graphics);
        }

        public void Shift(float height)
        {
            LayoutRectangle = new RectangleF(
                LayoutRectangle.X, LayoutRectangle.Y + height,
                LayoutRectangle.Width, LayoutRectangle.Height);

            Words.ForEach(word => word.LineShift(height));
        }

        internal static float GetDefaultSeparatorWidth(Graphics graphics, Font font) =>
            graphics.MeasureString(Separator, font).Width;

        internal static SizeF Measure(Graphics graphics, List<Word> words, Font font) =>
            Measure(words, font, GetDefaultSeparatorWidth(graphics, font));

        internal static SizeF Measure(List<Word> words, Font font, float separatorWidth)
        {
            var wordsWidth = words.Aggregate((float)0, (width, word) => width += word.Size.Width);
            var separatorsWidth = (words.Count - 1) * separatorWidth;

            return new SizeF(wordsWidth + separatorsWidth, font.Height);
        }

        public override string ToString() =>
            string.Join(Separator, Words);

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
                foreach (var word in Words)
                    word.Dispose();
            }

            disposed = true;
        }

        private List<Word> GetWords(IIconProvider iconProvider, Graphics graphics, string content,
            Font font, Color color, Color shadowColor, int shadowSize, int shortestAloneWords, RectangleF layoutRectangle)
        {
            var words = new List<Word>();
            var wordContents = content.Split(Separator);

            if (!wordContents.Any())
                return words;

            var wordLayoutPoint = new PointF(layoutRectangle.X, layoutRectangle.Y);
            foreach (var wordContent in wordContents)
            {
                var word = new Word(iconProvider, graphics, wordContent, font, color, shadowColor, shadowSize, shortestAloneWords, wordLayoutPoint);
                words.Add(word);
            }

            return words;
        }

        private float GetSeparatorWidth(Graphics graphics, Font font)
        {
            if (StringFormat.Alignment != StringAlignmentExtended.Justify || Words.Count < 2 || IsLast)
                return GetDefaultSeparatorWidth(graphics, font);

            var wordsWidth = Words.Aggregate((float)0, (width, word) => width += word.Size.Width);

            return (LayoutRectangle.Width - wordsWidth) / (Words.Count - 1);
        }

        private SizeF GetSize(Font font)
        {
            var size = new SizeF(0, font.Height);

            if (!Words.Any())
                return size;

            var contentWidth = Measure(Words, font, SeparatorWidth).Width;
            var shift = GetFirstShift(StringFormat.Alignment, contentWidth);
            size.Width = contentWidth + shift;

            foreach (var word in Words)
            {
                word.Shift(shift);
                shift += SeparatorWidth + word.Size.Width;
            }

            return size;
        }

        private float GetFirstShift(StringAlignmentExtended alignment, float contentWidth) =>
            alignment switch
            {
                StringAlignmentExtended.Center => (LayoutRectangle.Width - contentWidth) / 2,
                StringAlignmentExtended.Far => LayoutRectangle.Width - contentWidth,
                _ => 0
            };
    }
}

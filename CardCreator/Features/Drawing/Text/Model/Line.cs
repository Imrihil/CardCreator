using CardCreator.Features.Drawing.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CardCreator.Features.Drawing.Text.Model
{
    public class Line : IDisposable, IDrawable
    {
        private const string Separator = " ";

        private List<Word> Words { get; }
        public StringFormatExtended StringFormat { get; }
        public RectangleF LayoutRectangle { get; }
        public SizeF Size { get; }
        private SizeF SeparatorSize { get; }

        private bool disposed = false;

        //public Line(Graphics graphics, List<Word> words, StringFormatExtended stringFormat, RectangleF layoutRectangle)
        //{
        //    StringFormat = stringFormat;
        //    LayoutRectangle = layoutRectangle;
        //    Words = words;
        //}

        public Line(IIconProvider iconProvider, Graphics graphics, string content, StringFormatExtended stringFormat, Font font, Color color, Color shadowColor, int shadowSize, int shortestAloneWords, RectangleF layoutRectangle)
        {
            StringFormat = stringFormat;
            LayoutRectangle = layoutRectangle;
            Words = GetWords(iconProvider, graphics, content, font, color, shadowColor, shadowSize, shortestAloneWords, layoutRectangle);
            SeparatorSize = GetSeparatorSize(graphics, font);
            Size = GetSize(font);
        }

        private List<Word> GetWords(IIconProvider iconProvider, Graphics graphics, string content, Font font, Color color, Color shadowColor, int shadowSize, int shortestAloneWords, RectangleF layoutRectangle)
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

        private SizeF GetSeparatorSize(Graphics graphics, Font font)
        {
            if (StringFormat.Alignment != StringAlignmentExtended.Justify || Words.Count < 2)
                return graphics.MeasureString(Separator, font);

            var wordsWidth = Words.Aggregate((float)0, (width, word) => width += word.Size.Width);

            return new SizeF((LayoutRectangle.Width - wordsWidth) / (Words.Count - 1), font.Height);
        }

        private SizeF GetSize(Font font)
        {
            var size = new SizeF(0, font.Height);

            if (!Words.Any())
                return size;

            var wordsWidth = Words.Aggregate((float)0, (width, word) => width += word.Size.Width);
            var separatorWidth = (Words.Count - 1) * SeparatorSize.Width;
            var shift = GetFirstShift(StringFormat.Alignment, wordsWidth, separatorWidth);

            foreach (var word in Words)
            {
                word.Shift(shift);
                shift.X += SeparatorSize.Width + word.Size.Width;
            }

            return size;
        }

        private PointF GetFirstShift(StringAlignmentExtended alignment, float wordsWidth, float separatorWidth) =>
            alignment switch
            {
                StringAlignmentExtended.Center => new PointF((LayoutRectangle.Width - wordsWidth - separatorWidth) / 2, 0),
                StringAlignmentExtended.Far => new PointF(LayoutRectangle.Width - wordsWidth - separatorWidth, 0),
                _ => new PointF(0, 0)
            };

        private SizeF Measure(Graphics graphics, Font font)
        {
            if (!Words.Any())
                return new SizeF(0, font.Height);

            var size = Words.First().Size;
            foreach (var word in Words.Skip(1))
                size.Width += SeparatorSize.Width + word.Size.Width;

            return size;
        }

        private SizeF MeasureWithWord(Word word, Graphics graphics, Font font)
        {
            var size = Measure(graphics, font);
            size.Width += graphics.MeasureString(Separator, font).Width + word.Measure(graphics, font).Width;

            return size;
        }

        private static SizeF MeasureWithWord(Word word, Graphics graphics, Font font, SizeF sizeWithoutWord) =>
            new SizeF(
                sizeWithoutWord.Width + graphics.MeasureString(Separator, font).Width + word.Measure(graphics, font).Width,
                sizeWithoutWord.Height);

        private Line TrimAloneWords(int shortestAloneWords)
        {
            var newLine = new Line();
            for (var i = Count - 1; i > 1; i--)
            {
                var word = this[i];
                if (word.IsAlone(shortestAloneWords))
                    newLine.Add(word);
            }
            RemoveRange(Count - newLine.Count, newLine.Count);

            return newLine;
        }

        public void Draw(Graphics graphics)
        {
            foreach (var word in Words)
                word.Draw(graphics);
        }

        public bool TryAdd(Word word, float maxWidth, Graphics graphics, Font font)
        {
            var sizeWithoutWord = Measure(graphics, font);
            var size = MeasureWithWord(word, graphics, font, sizeWithoutWord);
            if (!this.Any() || size.Width <= maxWidth)
            {
                Add(word);
                return true;
            }

            return false;
        }

        public override string ToString() =>
            string.Join(Separator, this);

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
    }
}

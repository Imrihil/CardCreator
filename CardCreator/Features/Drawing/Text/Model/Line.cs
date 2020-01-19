using CardCreator.Features.Drawing.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CardCreator.Features.Drawing.Text.Model
{
    public class Line : List<Word>, IDisposable
    {
        private const string Separator = " ";

        private bool disposed = false;

        public Line() : base(new List<Word>()) { }
        public Line(IEnumerable<Word> words) : base(words) { }
        public Line(IIconProvider iconProvider, string content) : base(InitializeWords(iconProvider, content)) { }

        public static SizeF GetSeparatorSize(Graphics graphics, Font font) =>
            graphics.MeasureString(Separator, font);

        public SizeF Measure(Graphics graphics, Font font)
        {
            if (!this.Any())
                return new SizeF(0, font.Height);

            var size = this.First().Measure(graphics, font);
            var separatorSize = graphics.MeasureString(Separator, font);
            foreach (var word in this)
                size.Width += separatorSize.Width + word.Measure(graphics, font).Width;

            return size;
        }

        public SizeF MeasureWithWord(Word word, Graphics graphics, Font font)
        {
            var size = Measure(graphics, font);
            size.Width += graphics.MeasureString(Separator, font).Width + word.Measure(graphics, font).Width;

            return size;
        }

        public static SizeF MeasureWithWord(Word word, Graphics graphics, Font font, SizeF sizeWithoutWord) =>
            new SizeF(
                sizeWithoutWord.Width + graphics.MeasureString(Separator, font).Width + word.Measure(graphics, font).Width,
                sizeWithoutWord.Height);

        public Line TrimAloneWords(int shortestAloneWords)
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

        internal void Draw(Graphics graphics, StringFormatExtended stringFormat, Font font, Color color, Color shadowColor, int shadowSize, RectangleF layoutRectangle)
        {
            graphics.DrawStringWithShadow(ToString(), font, color, shadowColor, shadowSize, layoutRectangle, stringFormat.StringFormat);
            //var separatorWidth = stringFormat.Alignment == StringAlignmentExtended.Justify ? 1 : graphics.MeasureString(Separator, font).Width;
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
                foreach (var word in this)
                    word.Dispose();
            }

            disposed = true;
        }

        private static IEnumerable<Word> InitializeWords(IIconProvider iconProvider, string content) =>
            content.Split(Separator).Select(wordContent => new Word(iconProvider, wordContent));
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace CardCreator.Features.Drawing.Text.Model
{
    public class Line : List<Word>, IDisposable
    {
        private bool disposed = false;

        public Line(IIconProvider iconProvider, string content) : base(InitializeWords(iconProvider, content)) { }

        private static IEnumerable<Word> InitializeWords(IIconProvider iconProvider, string content) =>
            content.Split(' ').Select(wordContent => new Word(iconProvider, wordContent));

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

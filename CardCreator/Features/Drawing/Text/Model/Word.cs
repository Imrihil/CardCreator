using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace CardCreator.Features.Drawing.Text.Model
{
    public sealed class Word : IDisposable
    {
        public string Content { get; }
        public Image Icon { get; }

        private bool disposed = false;
        private string wordContent;

        public Word(IIconProvider iconProvider, string content)
        {
            Content = content;
            Icon = iconProvider.TryGet(content);
        }

        public bool IsAlone(int shortestAloneWords) =>
            Icon != null ? false : Content.Length < shortestAloneWords;

        public SizeF Measure(Graphics graphics, Font font) =>
            Icon == null ? graphics.MeasureString(Content, font) : Icon.Measure(font);

        public override string ToString() => Content;

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
                Icon?.Dispose();

            disposed = true;
        }
    }
}

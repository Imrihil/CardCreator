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
        public Font Font { get; }
        public Color Color { get; }
        public Color ShadowColor { get; }
        public int ShadowSize { get; }

        private bool disposed = false;
        private string wordContent;

        public Word(IIconProvider iconProvider, string content, Font font, Color color, Color shadowColor, int shadowSize)
        {
            Content = content;
            Icon = iconProvider.TryGet(content);
            Font = font;
            Color = color;
            ShadowColor = shadowColor;
            ShadowSize = shadowSize;
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

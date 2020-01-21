using CardCreator.Features.Drawing.Model;
using System;
using System.Drawing;

namespace CardCreator.Features.Drawing.Text.Model
{
    public sealed class Word : IDisposable, IDrawable
    {
        public static readonly StringFormatExtended DefaultStringFormat = new StringFormatExtended(StringAlignmentExtended.Near, StringAlignmentExtended.Near);

        public string Content { get; }
        public Image Icon { get; }
        public Font Font { get; }
        public Color Color { get; }
        public Color ShadowColor { get; }
        public int ShadowSize { get; }
        public bool IsAloneWord { get; }
        public RectangleF LayoutRectangle { get; private set; }
        public SizeF Size { get; }

        private bool disposed = false;

        public Word(IIconProvider iconProvider, Graphics graphics, string content, Font font, Color color, Color shadowColor, int shadowSize, int shortestAloneWords, PointF layoutPoint)
        {
            Content = content;
            Icon = iconProvider.TryGet(content);
            Font = font;
            Color = color;
            ShadowColor = shadowColor;
            ShadowSize = shadowSize;
            IsAloneWord = Icon != null ? false : Content.Length < shortestAloneWords;
            Size = GetSize(graphics);
            LayoutRectangle = new RectangleF(layoutPoint.X, layoutPoint.Y, Size.Width, Size.Height);
        }

        private SizeF GetSize(Graphics graphics) =>
            Icon == null ? graphics.MeasureString(Content, Font) : Icon.Measure(Font);

        public void Draw(Graphics graphics)
        {
            if (Icon != null)
                graphics.DrawImage(Icon, LayoutRectangle, DefaultStringFormat);
            else
                graphics.DrawStringWithShadow(ToString(), Font, Color, ShadowColor, ShadowSize, LayoutRectangle, DefaultStringFormat.StringFormat);
        }

        public void Shift(PointF shiftPoint)
        {
            LayoutRectangle = new RectangleF(LayoutRectangle.X + shiftPoint.X, LayoutRectangle.Y + shiftPoint.Y, LayoutRectangle.Width, LayoutRectangle.Height);
        }

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

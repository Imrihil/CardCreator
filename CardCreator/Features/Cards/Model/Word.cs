using CardCreator.Features.Drawing;
using CardCreator.Features.Drawing.Model;
using System;
using System.Drawing;

namespace CardCreator.Features.Cards.Model
{
    public sealed class Word : IDisposable, IDrawable
    {
        public static readonly StringFormatExtended DefaultStringFormat =
            new StringFormatExtended(StringAlignmentExtended.Near, StringAlignmentExtended.Near);

        public Font Font { get; }
        public Color Color { get; }
        public Color ShadowColor { get; }
        public int ShadowSize { get; }
        public bool IsAloneWord { get; }
        public bool IsImage => Icon != null;
        public RectangleF LayoutRectangle { get; private set; }
        public SizeF Size { get; }
        private string Content { get; }
        private Image Icon { get; }

        private bool disposed = false;

        public Word(IIconProvider iconProvider, Graphics graphics, string content,
            Font font, Color color, Color shadowColor, int shadowSize, int shortestAloneWords, PointF layoutPoint)
        {
            Content = content;
            Icon = iconProvider.TryGet(content);
            Font = font;
            Color = color;
            ShadowColor = shadowColor;
            ShadowSize = shadowSize;
            IsAloneWord = IsImage ? false : Content.Length < shortestAloneWords;

            Size = GetSize(graphics);
            LayoutRectangle = new RectangleF(layoutPoint.X, layoutPoint.Y, Size.Width, Size.Height);
        }

        public void Draw(Graphics graphics)
        {
            if (IsImage)
                graphics.DrawImage(Icon, LayoutRectangle, DefaultStringFormat);
            else
                graphics.DrawStringWithShadow(ToString(), Font, Color, ShadowColor, ShadowSize, LayoutRectangle, DefaultStringFormat.StringFormat);
        }

        public void LineShift(float height)
        {
            LayoutRectangle = new RectangleF(
                LayoutRectangle.X, LayoutRectangle.Y + height,
                LayoutRectangle.Width, LayoutRectangle.Height);
        }

        public void Shift(float width)
        {
            LayoutRectangle = new RectangleF(
                LayoutRectangle.X + width, LayoutRectangle.Y,
                LayoutRectangle.Width, LayoutRectangle.Height);
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

        private SizeF GetSize(Graphics graphics) =>
            IsImage ? Icon.Measure(Font) : graphics.MeasureString(Content, Font);
    }
}

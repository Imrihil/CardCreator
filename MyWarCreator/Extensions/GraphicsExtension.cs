using System.Collections.Generic;
using System.Drawing;
using System.Windows.Documents;
using MyWarCreator.Helpers;

namespace MyWarCreator.Extensions
{
    public static class GraphicsExtensions
    {
        public static void DrawStringWithShadow(this Graphics graphics, string s, Font font, Color color, RectangleF layoutRectangle, int shadowSize)
        {
            for (var offset = 0; offset <= shadowSize; ++offset)
            {
                using (Brush brush = new SolidBrush(Color.FromArgb(color.A * (shadowSize - offset) / (shadowSize + 1), color)))
                {
                    using (var shadowFont = new Font(font.FontFamily, font.Size + offset * 2, font.Style, font.Unit))
                        graphics.DrawString(s, shadowFont, brush, layoutRectangle);
                }
            }
        }

        public static void DrawStringWithShadow(this Graphics graphics, string s, Font font, Color color, RectangleF layoutRectangle, StringFormat format, int shadowSize)
        {
            for (var offset = 0; offset <= shadowSize; ++offset)
            {
                using (Brush brush = new SolidBrush(Color.FromArgb(color.A * (shadowSize - offset) / (shadowSize + 1), color)))
                {
                    using (var shadowFont = new Font(font.FontFamily, font.Size + offset * 2, font.Style, font.Unit))
                        graphics.DrawString(s, shadowFont, brush, layoutRectangle, format);
                }
            }
        }

        public static void DrawAdjustedString(this Graphics graphics, string s, Font font, Brush brush, RectangleF layoutRectangle, int minFontSize = 0, int maxFontSize = int.MinValue, bool smallestOnFail = true, bool wordWrap = true)
        {
            if (maxFontSize == int.MinValue) maxFontSize = (int)font.Size;
            using (var usedFont = FontsHelper.GetAdjustedFont(graphics, s, font, layoutRectangle, new StringFormat(), minFontSize, maxFontSize, smallestOnFail, wordWrap))
                graphics.DrawString(s, usedFont, brush, layoutRectangle);
        }

        public static void DrawAdjustedString(this Graphics graphics, string s, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format, int minFontSize = 0, int maxFontSize = int.MinValue, bool smallestOnFail = true, bool wordWrap = true)
        {
            if (maxFontSize == int.MinValue) maxFontSize = (int)font.Size;
            using (var usedFont = FontsHelper.GetAdjustedFont(graphics, s, font, layoutRectangle, format, minFontSize, maxFontSize, smallestOnFail, wordWrap))
                graphics.DrawString(s, usedFont, brush, layoutRectangle, format);
        }

        private static readonly List<Point> CloseBorderModifiers = new List<Point>
        {
            new Point(-1, -1),
            new Point(-1, 1),
            new Point(1, -1),
            new Point(1, 1)
        };

        private static readonly List<Point> ExtendedBorderModifiers = new List<Point>
        {
            new Point(-2, -2),
            new Point(-3, 0),
            new Point(-2, 2),
            new Point(0, -3),
            new Point(0, 3),
            new Point(2, -2),
            new Point(3, 0),
            new Point(2, 2)
        };

        public static void DrawAdjustedStringWithBorder(this Graphics graphics, string s, Font font, Brush brush, Brush brushBorder, RectangleF layoutRectangle, StringFormat format, int minFontSize = 0, int maxFontSize = int.MinValue, bool smallestOnFail = true, bool wordWrap = true)
        {
            if (maxFontSize == int.MinValue) maxFontSize = (int)font.Size;
            using (var usedFont = FontsHelper.GetAdjustedFont(graphics, s, font, layoutRectangle, format, minFontSize, maxFontSize, smallestOnFail, wordWrap))
            {
                foreach (var offset in CloseBorderModifiers)
                {
                    var layoutRectangleModified = new RectangleF(layoutRectangle.X + offset.X, layoutRectangle.Y + offset.Y, layoutRectangle.Width, layoutRectangle.Height);
                    graphics.DrawString(s, usedFont, brushBorder, layoutRectangleModified, format);
                }
            }
            using (var usedFont = FontsHelper.GetAdjustedFont(graphics, s, font, layoutRectangle, format, minFontSize, maxFontSize, smallestOnFail, wordWrap))
                graphics.DrawString(s, usedFont, brush, layoutRectangle, format);
        }

        public static void DrawAdjustedStringWithExtendedBorder(this Graphics graphics, string s, Font font, Color color, Color colorBorder, RectangleF layoutRectangle, StringFormat format, int minFontSize = 0, int maxFontSize = int.MinValue, bool smallestOnFail = true, bool wordWrap = true)
        {
            if (maxFontSize == int.MinValue) maxFontSize = (int)font.Size;
            using (var usedFont = FontsHelper.GetAdjustedFont(graphics, s, font, layoutRectangle, format, minFontSize, maxFontSize, smallestOnFail, wordWrap))
            {
                using (Brush brush = new SolidBrush(Color.FromArgb((int)((float)colorBorder.A * 4 / 5), colorBorder)))
                {
                    foreach (var offset in CloseBorderModifiers)
                    {
                        var layoutRectangleModified = new RectangleF(layoutRectangle.X + offset.X,
                            layoutRectangle.Y + offset.Y, layoutRectangle.Width, layoutRectangle.Height);
                        graphics.DrawString(s, usedFont, brush, layoutRectangleModified, format);
                    }
                }

                using (Brush brush = new SolidBrush(Color.FromArgb((int)((float)colorBorder.A / 4), colorBorder)))
                {
                    foreach (var offset in ExtendedBorderModifiers)
                    {
                        var layoutRectangleModified = new RectangleF(layoutRectangle.X + offset.X,
                            layoutRectangle.Y + offset.Y, layoutRectangle.Width, layoutRectangle.Height);
                        graphics.DrawString(s, usedFont, brush, layoutRectangleModified, format);
                    }
                }
            }
            using (var usedFont = FontsHelper.GetAdjustedFont(graphics, s, font, layoutRectangle, format, minFontSize, maxFontSize, smallestOnFail, wordWrap))
            using (Brush brush = new SolidBrush(color))
                graphics.DrawString(s, usedFont, brush, layoutRectangle, format);
        }
    }
}

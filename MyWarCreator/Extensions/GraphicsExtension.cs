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

        public static void DrawAdjustedStringWithBorder(this Graphics graphics, string s, Font font, Brush brush, Brush brushBorder, RectangleF layoutRectangle, StringFormat format, int minFontSize = 0, int maxFontSize = int.MinValue, bool smallestOnFail = true, bool wordWrap = true)
        {
            if (maxFontSize == int.MinValue) maxFontSize = (int)font.Size;
            var layoutRectangleTopLeft = new RectangleF(layoutRectangle.X - 1, layoutRectangle.Y - 1, layoutRectangle.Width, layoutRectangle.Height);
            var layoutRectangleTopRight = new RectangleF(layoutRectangle.X - 1, layoutRectangle.Y + 1, layoutRectangle.Width, layoutRectangle.Height);
            var layoutRectangleBottomLeft = new RectangleF(layoutRectangle.X + 1, layoutRectangle.Y - 1, layoutRectangle.Width, layoutRectangle.Height);
            var layoutRectangleBottomRight = new RectangleF(layoutRectangle.X + 1, layoutRectangle.Y + 1, layoutRectangle.Width, layoutRectangle.Height);
            using (var usedFont = FontsHelper.GetAdjustedFont(graphics, s, font, layoutRectangle, format, minFontSize, maxFontSize, smallestOnFail, wordWrap))
            {
                graphics.DrawString(s, usedFont, brushBorder, layoutRectangleTopLeft, format);
                graphics.DrawString(s, usedFont, brushBorder, layoutRectangleTopRight, format);
                graphics.DrawString(s, usedFont, brushBorder, layoutRectangleBottomLeft, format);
                graphics.DrawString(s, usedFont, brushBorder, layoutRectangleBottomRight, format);
            }
            using (var usedFont = FontsHelper.GetAdjustedFont(graphics, s, font, layoutRectangle, format, minFontSize, maxFontSize, smallestOnFail, wordWrap))
                graphics.DrawString(s, usedFont, brush, layoutRectangle, format);
        }

        public static void DrawAdjustedStringWithExtendedBorder(this Graphics graphics, string s, Font font, Brush brush, Brush brushBorder, RectangleF layoutRectangle, StringFormat format, int minFontSize = 0, int maxFontSize = int.MinValue, bool smallestOnFail = true, bool wordWrap = true)
        {
            if (maxFontSize == int.MinValue) maxFontSize = (int)font.Size;
            var layoutRectangleTopLeft = new RectangleF(layoutRectangle.X - 1, layoutRectangle.Y - 1, layoutRectangle.Width, layoutRectangle.Height);
            var layoutRectangleTopRight = new RectangleF(layoutRectangle.X - 1, layoutRectangle.Y + 1, layoutRectangle.Width, layoutRectangle.Height);
            var layoutRectangleBottomLeft = new RectangleF(layoutRectangle.X + 1, layoutRectangle.Y - 1, layoutRectangle.Width, layoutRectangle.Height);
            var layoutRectangleBottomRight = new RectangleF(layoutRectangle.X + 1, layoutRectangle.Y + 1, layoutRectangle.Width, layoutRectangle.Height);
            using (var usedFont = FontsHelper.GetAdjustedFont(graphics, s, font, layoutRectangle, format, minFontSize, maxFontSize, smallestOnFail, wordWrap))
            {
                graphics.DrawString(s, usedFont, brushBorder, layoutRectangleTopLeft, format);
                graphics.DrawString(s, usedFont, brushBorder, layoutRectangleTopRight, format);
                graphics.DrawString(s, usedFont, brushBorder, layoutRectangleBottomLeft, format);
                graphics.DrawString(s, usedFont, brushBorder, layoutRectangleBottomRight, format);
            }
            using (var usedFont = FontsHelper.GetAdjustedFont(graphics, s, font, layoutRectangle, format, minFontSize, maxFontSize, smallestOnFail, wordWrap))
                graphics.DrawString(s, usedFont, brush, layoutRectangle, format);
        }
    }
}

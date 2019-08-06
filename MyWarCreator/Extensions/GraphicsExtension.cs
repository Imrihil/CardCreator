using System.Drawing;
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
    }
}

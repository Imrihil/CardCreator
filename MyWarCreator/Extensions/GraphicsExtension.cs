using MyWarCreator.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWarCreator.Extensions
{
    public static class GraphicsExtensions
    {
        public static void DrawStringWithShadow(this Graphics graphics, string s, Font font, Color color, RectangleF layoutRectangle, int shadowSize)
        {
            for (int offset = 0; offset <= shadowSize; ++offset)
            {
                using (Brush brush = new SolidBrush(Color.FromArgb(color.A * (shadowSize - offset) / (shadowSize + 1), color)))
                {
                    using (Font shadowFont = new Font(font.FontFamily, font.Size + offset * 2, font.Style, font.Unit))
                        graphics.DrawString(s, shadowFont, brush, layoutRectangle);
                }
            }
        }

        public static void DrawStringWithShadow(this Graphics graphics, string s, Font font, Color color, RectangleF layoutRectangle, StringFormat format, int shadowSize)
        {
            for (int offset = 0; offset <= shadowSize; ++offset)
            {
                using (Brush brush = new SolidBrush(Color.FromArgb(color.A * (shadowSize - offset) / (shadowSize + 1), color)))
                {
                    using (Font shadowFont = new Font(font.FontFamily, font.Size + offset * 2, font.Style, font.Unit))
                        graphics.DrawString(s, shadowFont, brush, layoutRectangle, format);
                }
            }
        }

        public static void DrawAdjustedString(this Graphics graphics, string s, Font font, Brush brush, RectangleF layoutRectangle, int minFontSize = 0, int maxFontSize = int.MinValue, bool smallestOnFail = true, bool wordWrap = true)
        {
            if (maxFontSize == int.MinValue) maxFontSize = (int)font.Size;
            using (Font usedFont = FontsHelper.GetAdjustedFont(graphics, s, font, layoutRectangle, new StringFormat(), minFontSize, maxFontSize, smallestOnFail, wordWrap))
                graphics.DrawString(s, usedFont, brush, layoutRectangle);
        }

        public static void DrawAdjustedString(this Graphics graphics, string s, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format, int minFontSize = 0, int maxFontSize = int.MinValue, bool smallestOnFail = true, bool wordWrap = true)
        {
            if (maxFontSize == int.MinValue) maxFontSize = (int)font.Size;
            using (Font usedFont = FontsHelper.GetAdjustedFont(graphics, s, font, layoutRectangle, format, minFontSize, maxFontSize, smallestOnFail, wordWrap))
                graphics.DrawString(s, usedFont, brush, layoutRectangle, format);
        }
    }
}

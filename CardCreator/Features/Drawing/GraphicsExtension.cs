using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace MyWarCreator.Extensions
{
    public static class GraphicsExtensions
    {
        private static readonly List<Point> CloseBorderModifiers = new List<Point>
        {
            new Point(-1, -1),
            new Point(-1, 1),
            new Point(1, -1),
            new Point(1, 1)
        };

        private static readonly List<Point> ShadowModifiers = new List<Point>
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

        public static void DrawAdjustedString(this Graphics graphics, string s, FontFamily fontFamily, Color color, RectangleF layoutRectangle, int maxFontSize, StringFormat format = default, int minFontSize = 0, bool smallestOnFail = true, bool wordWrap = true)
        {
            using var font = graphics.GetAdjustedFont(s, fontFamily, layoutRectangle, format, maxFontSize, minFontSize, smallestOnFail, wordWrap);
            using var brush = new SolidBrush(color);
            graphics.DrawString(s, font, brush, layoutRectangle, format);
        }

        public static void DrawAdjustedStringWithShadow(this Graphics graphics, string s, FontFamily fontFamily, Color color, Color colorBorder, RectangleF layoutRectangle, int maxFontSize, StringFormat format = default, int minFontSize = 0, bool smallestOnFail = true, bool wordWrap = true)
        {
            using var font = graphics.GetAdjustedFont(s, fontFamily, layoutRectangle, format, maxFontSize, minFontSize, smallestOnFail, wordWrap);
            graphics.DrawStringWithShadow(s, font, color, colorBorder, layoutRectangle, format);
        }

        public static void DrawStringWithShadow(this Graphics graphics, string s, Font font, Color color, Color colorBorder, RectangleF layoutRectangle, StringFormat format = default)
        {
            using (Brush brush = new SolidBrush(Color.FromArgb((int)((float)colorBorder.A * 4 / 5), colorBorder)))
            {
                foreach (var offset in CloseBorderModifiers)
                {
                    var layoutRectangleModified = new RectangleF(layoutRectangle.X + offset.X,
                        layoutRectangle.Y + offset.Y, layoutRectangle.Width, layoutRectangle.Height);
                    graphics.DrawString(s, font, brush, layoutRectangleModified, format);
                }
            }

            using (Brush brush = new SolidBrush(Color.FromArgb((int)((float)colorBorder.A / 4), colorBorder)))
            {
                foreach (var offset in ShadowModifiers)
                {
                    var layoutRectangleModified = new RectangleF(layoutRectangle.X + offset.X,
                        layoutRectangle.Y + offset.Y, layoutRectangle.Width, layoutRectangle.Height);
                    graphics.DrawString(s, font, brush, layoutRectangleModified, format);
                }
            }

            using (Brush brush = new SolidBrush(color))
                graphics.DrawString(s, font, brush, layoutRectangle, format);
        }

        public static void DrawAdjustedStringWithBorder(this Graphics graphics, string s, FontFamily fontFamily, Color color, Color borderColor, RectangleF layoutRectangle, int maxFontSize, StringFormat format = default, int minFontSize = 0, bool smallestOnFail = true, bool wordWrap = true)
        {
            using var font = graphics.GetAdjustedFont(s, fontFamily, layoutRectangle, format, maxFontSize, minFontSize, smallestOnFail, wordWrap);
            graphics.DrawStringWithBorder(s, font, color, borderColor, layoutRectangle, format);
        }

        public static void DrawStringWithBorder(this Graphics graphics, string s, Font font, Color color, Color borderColor, RectangleF layoutRectangle, StringFormat format = default)
        {
            using var brush = new SolidBrush(color);
            using var borderBrush = new SolidBrush(borderColor);

            foreach (var offset in CloseBorderModifiers)
            {
                var layoutRectangleModified = new RectangleF(layoutRectangle.X + offset.X, layoutRectangle.Y + offset.Y, layoutRectangle.Width, layoutRectangle.Height);
                graphics.DrawString(s, font, borderBrush, layoutRectangleModified, format);
            }
            graphics.DrawString(s, font, brush, layoutRectangle, format);
        }

        public static Font GetAdjustedFont(this Graphics graphics, string graphicString, FontFamily fontFamily, RectangleF container, StringFormat stringFormat, int maxFontSize, int minFontSize, bool smallestOnFail = true, bool wordWrap = true)
        {
            // We utilize MeasureString which we get via a control instance           
            for (var adjustedSize = maxFontSize; adjustedSize >= minFontSize; adjustedSize--)
            {
                var testFont = new Font(fontFamily, adjustedSize, GraphicsUnit.Pixel);

                // Test the string with the new size
                var adjustedSizeNew = graphics.MeasureString(graphicString, testFont, new SizeF(container.Width, container.Height), stringFormat, out var characterFitted, out var linesFilled);

                if (characterFitted == graphicString.Length && (wordWrap || linesFilled == graphicString.Count(x => x == '\n') + 1) && container.Width > Convert.ToInt32(adjustedSizeNew.Width) && container.Height > Convert.ToInt32(adjustedSizeNew.Height))
                {
                    // Good font, return it
                    return testFont;
                }
            }

            // If you get here there was no font size that worked
            // return MinimumSize or Original?
            return smallestOnFail
                ? new Font(fontFamily, minFontSize, GraphicsUnit.Pixel)
                : new Font(fontFamily, maxFontSize, GraphicsUnit.Pixel);
        }
    }
}

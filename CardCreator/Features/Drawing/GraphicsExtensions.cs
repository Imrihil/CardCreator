using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace CardCreator.Features.Drawing
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

        public static void DrawAdjustedStringWithShadow(this Graphics graphics, string s, FontFamily fontFamily, Color color, Color shadowColor, int shadowSize, RectangleF layoutRectangle, int maxFontSize, StringFormat format = default, int minFontSize = 0, bool smallestOnFail = true, bool wordWrap = true)
        {
            switch (shadowSize)
            {
                case 0:
                    graphics.DrawAdjustedString(s, fontFamily, color, layoutRectangle, maxFontSize, format, minFontSize, smallestOnFail, wordWrap);
                    break;
                case 1:
                    graphics.DrawAdjustedStringWithBorder(s, fontFamily, color, shadowColor, layoutRectangle, maxFontSize, format, minFontSize, smallestOnFail, wordWrap);
                    break;
                default:
                    graphics.DrawAdjustedStringWithShadow(s, fontFamily, color, shadowColor, layoutRectangle, maxFontSize, format, minFontSize, smallestOnFail, wordWrap);
                    break;
            }
        }

        public static void DrawStringWithShadow(this Graphics graphics, string s, Font font, Color color, Color shadowColor, int shadowSize, RectangleF layoutRectangle, StringFormat format = default)
        {
            switch (shadowSize)
            {
                case 0:
                    {
                        using var brush = new SolidBrush(color);
                        graphics.DrawString(s, font, brush, layoutRectangle, format);
                        break;
                    }
                case 1:
                    graphics.DrawStringWithBorder(s, font, color, shadowColor, layoutRectangle, format);
                    break;
                default:
                    graphics.DrawStringWithShadow(s, font, color, shadowColor, layoutRectangle, format);
                    break;
            }
        }

        private static void DrawAdjustedStringWithShadow(this Graphics graphics, string s, FontFamily fontFamily, Color color, Color shadowColor, RectangleF layoutRectangle, int maxFontSize, StringFormat format = default, int minFontSize = 0, bool smallestOnFail = true, bool wordWrap = true)
        {
            using var font = graphics.GetAdjustedFont(s, fontFamily, layoutRectangle, format, maxFontSize, minFontSize, smallestOnFail, wordWrap);
            graphics.DrawStringWithShadow(s, font, color, shadowColor, layoutRectangle, format);
        }

        internal static void DrawStringWithShadow(this Graphics graphics, string s, Font font, Color color, Color shadowColor, RectangleF layoutRectangle, StringFormat format = default)
        {
            using (Brush brush = new SolidBrush(Color.FromArgb((int)((float)shadowColor.A * 4 / 5), shadowColor)))
            {
                foreach (var offset in CloseBorderModifiers)
                {
                    var layoutRectangleModified = new RectangleF(layoutRectangle.X + offset.X,
                        layoutRectangle.Y + offset.Y, layoutRectangle.Width, layoutRectangle.Height);
                    graphics.DrawString(s, font, brush, layoutRectangleModified, format);
                }
            }

            using (Brush brush = new SolidBrush(Color.FromArgb((int)((float)shadowColor.A / 4), shadowColor)))
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

        private static void DrawAdjustedStringWithBorder(this Graphics graphics, string s, FontFamily fontFamily, Color color, Color shadowColor, RectangleF layoutRectangle, int maxFontSize, StringFormat format = default, int minFontSize = 0, bool smallestOnFail = true, bool wordWrap = true)
        {
            using var font = graphics.GetAdjustedFont(s, fontFamily, layoutRectangle, format, maxFontSize, minFontSize, smallestOnFail, wordWrap);
            graphics.DrawStringWithBorder(s, font, color, shadowColor, layoutRectangle, format);
        }

        private static void DrawStringWithBorder(this Graphics graphics, string s, Font font, Color color, Color shadowColor, RectangleF layoutRectangle, StringFormat format = default)
        {
            using var brush = new SolidBrush(color);
            using var borderBrush = new SolidBrush(shadowColor);

            foreach (var offset in CloseBorderModifiers)
            {
                var layoutRectangleModified = new RectangleF(layoutRectangle.X + offset.X, layoutRectangle.Y + offset.Y, layoutRectangle.Width, layoutRectangle.Height);
                graphics.DrawString(s, font, borderBrush, layoutRectangleModified, format);
            }
            graphics.DrawString(s, font, brush, layoutRectangle, format);
        }

        internal static Font GetAdjustedFont(this Graphics graphics, string s, FontFamily fontFamily, RectangleF layoutRectangle, StringFormat stringFormat, int maxFontSize, int minFontSize, bool smallestOnFail = true, bool wordWrap = true)
        {
            // We utilize MeasureString which we get via a control instance           
            for (var adjustedSize = maxFontSize; adjustedSize >= minFontSize; adjustedSize--)
            {
                var testFont = new Font(fontFamily, adjustedSize, GraphicsUnit.Pixel);

                // Test the string with the new size
                var adjustedSizeNew = graphics.MeasureString(s, testFont, new SizeF(layoutRectangle.Width, layoutRectangle.Height), stringFormat, out var characterFitted, out var linesFilled);

                if (characterFitted == s.Length && (wordWrap || linesFilled == s.Count(x => x == '\n') + 1) && layoutRectangle.Width > Convert.ToInt32(adjustedSizeNew.Width) && layoutRectangle.Height > Convert.ToInt32(adjustedSizeNew.Height))
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

        /// <summary>
        /// Draws the specified Image at the specified location and with the scaled size.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static void DrawImage(this Graphics graphics, Image image,
            Rectangle layoutRectangle, StringFormat stringFormat, bool stretch)
        {
            // Scale.
            // Get scale factors for both directions.
            var scaleX = (float)layoutRectangle.Width / image.Width;
            var scaleY = (float)layoutRectangle.Height / image.Height;

            if (!stretch)
            {
                // To preserve the aspect ratio,
                // use the smaller scale factor.
                scaleX = Math.Min(scaleX, scaleY);
                scaleY = scaleX;
            }

            var targetWidth = (int)(image.Width * scaleX);
            var targetHeight = (int)(image.Height * scaleY);

            if (targetWidth == layoutRectangle.Width && targetHeight == layoutRectangle.Height)
            {
                graphics.DrawImage(image, layoutRectangle);
            }
            else
            {
                using Image targetImage = image.Resize(targetWidth, targetHeight);

                var translateX =
                    stringFormat.Alignment == StringAlignment.Near ? 0 :
                    stringFormat.Alignment == StringAlignment.Center ? (layoutRectangle.Width - targetImage.Width) / 2 :
                    layoutRectangle.Width - targetImage.Width;
                var translateY =
                    stringFormat.LineAlignment == StringAlignment.Near ? 0 :
                    stringFormat.LineAlignment == StringAlignment.Center ? (layoutRectangle.Height - targetImage.Height) / 2 :
                    layoutRectangle.Height - targetImage.Height;

                graphics.DrawImage(targetImage, layoutRectangle.X + translateX, layoutRectangle.Y + translateY, targetWidth, targetHeight);
            }
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        private static Bitmap Resize(this Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            using var graphics = Graphics.FromImage(destImage);
            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);

            return destImage;
        }
    }
}

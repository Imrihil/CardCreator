using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CardCreator.Helpers
{
    public static class DrawingHelper
    {
        // Map a drawing coordinate rectangle to
        // a graphics object rectangle.
        public static void MapDrawing(Graphics graphic, Image image,
            Rectangle targetRect, bool stretch = false, bool center = true)
        {
            // Scale.
            // Get scale factors for both directions.
            var scaleX = (float)targetRect.Width / image.Width;
            var scaleY = (float)targetRect.Height / image.Height;
            if (!stretch)
            {
                // To preserve the aspect ratio,
                // use the smaller scale factor.
                scaleX = Math.Min(scaleX, scaleY);
                scaleY = scaleX;
            }

            using var img = ResizeImage(image, (int)(image.Width * scaleX), (int)(image.Height * scaleY));
            var translateX = center ? (targetRect.Width - img.Width) / 2 : 0;
            var translateY = center ? (targetRect.Height - img.Height) / 2 : 0;

            graphic.DrawImage(img, targetRect.X + translateX, targetRect.Y + translateY);
            //graphic.DrawImage(image, target_rect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        private static Bitmap ResizeImage(Image image, int width, int height)
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

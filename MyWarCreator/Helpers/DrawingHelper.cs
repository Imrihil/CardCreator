using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWarCreator.Helpers
{
    class DrawingHelper
    {
        // Map a drawing coordinate rectangle to
        // a graphics object rectangle.
        public static void MapDrawing(Graphics graphic, Image image,
            Rectangle target_rect, bool stretch = false)
        {
            // Scale.
            // Get scale factors for both directions.
            float scale_x = (float)target_rect.Width / image.Width;
            float scale_y = (float)target_rect.Height / image.Height;
            if (!stretch)
            {
                // To preserve the aspect ratio,
                // use the smaller scale factor.
                scale_x = Math.Min(scale_x, scale_y);
                scale_y = scale_x;
            }

            using (Image img = ResizeImage(image, (int)(image.Width * scale_x), (int)(image.Height * scale_y)))
            {
                int translate_x = (target_rect.Width - img.Width) / 2;
                int translate_y = (target_rect.Height - img.Height) / 2;

                graphic.DrawImage(img, target_rect.X + translate_x, target_rect.Y + translate_y);
            }
            //graphic.DrawImage(image, target_rect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            using (Graphics graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
            }

            return destImage;
        }
    }
}

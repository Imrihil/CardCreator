using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace CardCreator.Features.Drawing
{
    public static class ImageExtensions
    {
        public static Bitmap GetNewBitmap(this Image image)
        {
            lock (image)
            {
                return new Bitmap(image);
            }
        }

        public static BitmapImage ToBitmapImage(this Image image)
        {
            var stream = new MemoryStream();
            image.Save(stream, ImageFormat.Png);

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = stream;
            bitmapImage.EndInit();

            return bitmapImage;
        }

        public static SizeF Measure(this Image image, Font font) =>
            new SizeF((float)font.Height * image.Width / image.Height, font.Height);
    }
}

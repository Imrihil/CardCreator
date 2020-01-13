using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace CardCreator.Features.Preview
{
    public class Preview : IPreview
    {
        private string FilePath { get; }

        public Preview(string filePath)
        {
            FilePath = filePath;
        }

        public BitmapImage GetImage()
        {
            var image = new Bitmap(400, 550);
            using var graphics = Graphics.FromImage(image);
            graphics.FillRectangle(Brushes.White, 0, 0, 400, 550);
            graphics.DrawRectangle(Pens.Black, 0, 0, 400, 550);
            return BitmapImageFromImage(image);
        }

        public BitmapImage Next()
        {
            var image = new Bitmap(400, 550);
            using var graphics = Graphics.FromImage(image);
            graphics.FillRectangle(Brushes.White, 0, 0, 400, 550);
            graphics.DrawRectangle(Pens.Black, 0, 0, 300, 400);
            return BitmapImageFromImage(image);
        }

        public BitmapImage Previous()
        {
            var image = new Bitmap(400, 550);
            using var graphics = Graphics.FromImage(image);
            graphics.FillRectangle(Brushes.White, 0, 0, 400, 550);
            graphics.DrawRectangle(Pens.Black, 0, 0, 150, 300);
            return BitmapImageFromImage(image);
        }

        private static BitmapImage BitmapImageFromImage(Image image)
        {
            var stream = new MemoryStream();
            image.Save(stream, ImageFormat.Png);

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = stream;
            bitmapImage.EndInit();

            return bitmapImage;
        }
    }
}

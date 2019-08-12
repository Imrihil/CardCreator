using System.Drawing;
using System.IO;

namespace MyWarCreator.Helpers
{
    public static class ImageHelper
    {
        public static Image LoadImage(string dirPath, string name)
        {
            var imagePath = $"{dirPath}/{name}.png";
            if (!File.Exists(imagePath))
                imagePath = $"{dirPath}/{name}.jpg";
            if (!File.Exists(imagePath))
                imagePath = $"{dirPath}/{name.ToLower()}.png";
            if (!File.Exists(imagePath))
                imagePath = $"{dirPath}/{name.ToLower()}.jpg";
            return File.Exists(imagePath) ? Image.FromFile(imagePath) : null;
        }
    }
}

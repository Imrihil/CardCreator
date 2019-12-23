using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace CardCreator.Features.Images
{
    public class ImageProvider : IImageProvider
    {
        private readonly IDictionary<string, ImageStats> cacheCollection;
        private int MaxSize { get; } = 100;
        private int MaxTime { get; } = 60; // in seconds
        public DateTime ValidTime => DateTime.Now.AddSeconds(-MaxTime);

        public ImageProvider()
        {
            cacheCollection = new Dictionary<string, ImageStats>();
        }

        public Image Get(string name)
        {
            if (cacheCollection.TryGetValue(name, out var imageWithStats) && imageWithStats.Timestamp >= ValidTime)
            {
                return imageWithStats.GetImage();
            }

            if (File.Exists(name))
            {
                CleanCacheCollection();
                var image = Image.FromFile(name);
                imageWithStats = new ImageStats(image);
                cacheCollection[name] = imageWithStats;
                return image;
            }

            return null;
        }

        private void CleanCacheCollection()
        {
            if (cacheCollection.Count > MaxSize)
            {
                var oldest = cacheCollection.Where(kv => kv.Value.Timestamp < ValidTime);
                foreach (var image in oldest)
                {
                    cacheCollection.Remove(image.Key);
                }
            }
        }

        private class ImageStats
        {
            private Image Image { get; }
            public int RequestsNumber { get; private set; }
            public DateTime Timestamp { get; private set; }

            public ImageStats(Image image)
            {
                Image = image;
                RequestsNumber = 1;
                Timestamp = DateTime.Now;
            }

            public Image GetImage()
            {
                ++RequestsNumber;
                return Image;
            }
        }
    }
}

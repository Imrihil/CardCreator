using CardCreator.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace CardCreator.Features.Drawing
{
    public sealed class ImageProvider : IImageProvider, IDisposable
    {
        private readonly IDictionary<string, ImageStats> cacheCollection;
        private int MaxSize { get; } = 100;
        private int MaxTime { get; } = 10; // in seconds
        public DateTime ValidTime => DateTime.Now.AddSeconds(-MaxTime);

        private bool disposed = false;

        public ImageProvider(IOptions<AppSettings> settings)
        {
            cacheCollection = new Dictionary<string, ImageStats>();
            MaxTime = settings.Value.ImageCacheTimeout;
        }

        public Image Get(string name)
        {
            return TryGet(name) ?? throw new FileNotFoundException($"File with path {name} not exists.");
        }

        public Image TryGet(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            if (cacheCollection.TryGetValue(name, out var imageWithStats) && imageWithStats.Timestamp >= ValidTime)
            {
                return imageWithStats.Image.GetNewBitmap();
            }

            if (File.Exists(name))
            {
                CleanCacheCollection();
                using var bitmap = new Bitmap(name);
                var image = new Bitmap(bitmap);
                imageWithStats = new ImageStats(image);
                cacheCollection[name] = imageWithStats;
                return image.GetNewBitmap();
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
                    image.Value.Dispose();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
                foreach (var image in cacheCollection)
                    image.Value.Dispose();

            disposed = true;
        }

        private sealed class ImageStats : IDisposable
        {
            private readonly Image image;
            public Image Image
            {
                get
                {
                    ++RequestsNumber;
                    return image;
                }
            }
            public int RequestsNumber { get; private set; }
            public DateTime Timestamp { get; private set; }

            private bool disposed = false;

            public ImageStats(Image image)
            {
                this.image = image;
                RequestsNumber = 1;
                Timestamp = DateTime.Now;
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (disposed)
                    return;

                if (disposing)
                    image.Dispose();

                disposed = true;
            }
        }
    }
}

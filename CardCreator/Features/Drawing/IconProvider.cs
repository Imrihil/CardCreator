using CardCreator.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CardCreator.Features.Drawing
{
    public sealed class IconProvider : IIconProvider, IDisposable
    {
        private readonly Dictionary<string, Image> icons;
        private bool disposed = false;

        public IconProvider(IImageProvider imageProvider, IOptions<AppSettings> settings)
        {
            icons = new Dictionary<string, Image>(InitializeIcons(imageProvider, settings.Value.Icons));
        }

        public Image Get(string name) =>
            TryGet(name) ?? throw new KeyNotFoundException($"Icon with name {name} not exists.");

        public Image TryGet(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            return icons.TryGetValue(name, out var image) ?
                image :
                null;
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
            {
                foreach (var icon in icons.Values)
                    icon.Dispose();
            }

            disposed = true;
        }

        private IEnumerable<KeyValuePair<string, Image>> InitializeIcons(IImageProvider imageProvider, IDictionary<string, string> icons) =>
            icons.ToDictionary(kv => kv.Key, kv => imageProvider.TryGet(kv.Value)).Where(kv => kv.Value != null);
    }
}

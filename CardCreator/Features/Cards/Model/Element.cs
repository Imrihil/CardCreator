using CardCreator.Features.Drawing;
using CardCreator.Settings;
using System;
using System.Drawing;
using System.IO;

namespace CardCreator.Features.Cards.Model
{
    public sealed class Element : IDrawable, IDisposable
    {
        public string Content { get; }
        public ElementSchema ElementSchema { get; private set; }
        private Image Image { get; }
        private Article Article { get; }

        private bool disposed = false;

        public Element(TextSettings settings, IImageProvider imageProvider, IIconProvider iconProvider,
            string content, ElementSchema elementSchema, string directory, bool generateImages = true)
        {
            ElementSchema = elementSchema;
            if (string.IsNullOrEmpty(content))
                return;
            Content = content;
            Image = imageProvider.TryGet(Path.Combine(directory, content)) ?? imageProvider.TryGetImageFromColor(content, ElementSchema.Area.Width, ElementSchema.Area.Height);
            if (Image == null && elementSchema.MaxSize > 0 && ElementSchema.Area.Width > 0 && ElementSchema.Area.Height > 0)
            {
                using var tmpImage = new Bitmap(ElementSchema.Area.Width, ElementSchema.Area.Height);
                using var graphics = Graphics.FromImage(tmpImage);
                Article = new Article(iconProvider, graphics, content, ElementSchema.StringFormat, ElementSchema.FontFamily, ElementSchema.MaxSize, ElementSchema.MinSize,
                    ElementSchema.Color, ElementSchema.ShadowColor, ElementSchema.ShadowSize, ElementSchema.WrapLines, settings.ShortestAloneWords, ElementSchema.Area);
            }

            if (Image != null && !generateImages)
            {
                Content = null;
                Image = null;
            }
        }

        public void Draw(Graphics graphics)
        {
            if (ElementSchema.Area.Width == 0 && ElementSchema.Area.Height == 0)
                return;

            if (ElementSchema.Background != null)
            {
                using var background = ElementSchema.Background.GetNewBitmap();
                graphics.DrawImage(background, ElementSchema.Area);
            }

            if (Image != null)
            {
                using var image = Image.GetNewBitmap();
                graphics.DrawImage(image, ElementSchema.Area, ElementSchema.StringFormat);
            }
            else if (Article != null)
            {
                Article.Draw(graphics);
            }
            else if (!string.IsNullOrWhiteSpace(Content) && ElementSchema.MaxSize > 0)
            {
                graphics.DrawAdjustedStringWithShadow(Content, ElementSchema.FontFamily, ElementSchema.Color, ElementSchema.ShadowColor, ElementSchema.ShadowSize, ElementSchema.Area, ElementSchema.MaxSize, ElementSchema.StringFormat, ElementSchema.MinSize, true, ElementSchema.WrapLines);
            }
        }

        internal void SetPosition(int position, int all)
        {
            if (ElementSchema.JoinDirection == JoinDirection.None)
                return;

            var shift = ElementSchema.JoinDirection == JoinDirection.Horizontally ?
                ElementSchema.Area.Width / all :
                ElementSchema.Area.Height / all;

            var area = ElementSchema.JoinDirection == JoinDirection.Horizontally ?
                new Rectangle(ElementSchema.Area.X + position * shift, ElementSchema.Area.Y, shift, ElementSchema.Area.Height) :
                new Rectangle(ElementSchema.Area.X, ElementSchema.Area.Y + position * shift, ElementSchema.Area.Width, shift);

            ElementSchema = new ElementSchema(ElementSchema.Name, ElementSchema.Background, area, ElementSchema.Color, ElementSchema.ShadowColor, ElementSchema.ShadowSize, ElementSchema.FontFamily,
                ElementSchema.MaxSize, ElementSchema.StringFormat, ElementSchema.WrapLines, ElementSchema.JoinDirection);
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
                Image?.Dispose();
            }

            disposed = true;
        }
    }
}

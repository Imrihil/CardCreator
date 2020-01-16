﻿using CardCreator.Features.Drawing;
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

        private bool disposed = false;

        public Element(IImageProvider imageProvider, string content, ElementSchema elementSchema, string directory, bool generateImages = true)
        {
            Content = content;
            Image = imageProvider.TryGet(Path.Combine(directory, content));
            if (Image != null && !generateImages)
            {
                Content = null;
                Image = null;
            }
            ElementSchema = elementSchema;
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
            else if (!string.IsNullOrWhiteSpace(Content) && ElementSchema.MaxSize > 0)
            {
                graphics.DrawAdjustedStringWithShadow(Content, ElementSchema.Font, ElementSchema.Color, ElementSchema.ShadowColor, ElementSchema.ShadowSize, ElementSchema.Area, ElementSchema.MaxSize, ElementSchema.StringFormat, ElementSchema.MinSize, true, ElementSchema.Wrap);
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

            ElementSchema = new ElementSchema(ElementSchema.Name, ElementSchema.Background, area, ElementSchema.Color, ElementSchema.ShadowColor, ElementSchema.ShadowSize, ElementSchema.Font,
                ElementSchema.MaxSize, ElementSchema.StringFormat, ElementSchema.Wrap, ElementSchema.JoinDirection);
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

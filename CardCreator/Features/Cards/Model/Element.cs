using CardCreator.Features.Drawing;
using CardCreator.Features.Images;
using System.Drawing;
using System.IO;

namespace CardCreator.Features.Cards.Model
{
    public class Element : IDrawable
    {
        public string Content { get; }
        public ElementSchema ElementSchema { get; private set; }
        private Image Image { get; }

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
                graphics.DrawImage(ElementSchema.Background, ElementSchema.Area);

            if (Image != null)
                graphics.DrawImage(Image, ElementSchema.Area, ElementSchema.StringFormat.StringFormat, ElementSchema.StretchImage);
            else if (!string.IsNullOrWhiteSpace(Content))
                graphics.DrawAdjustedStringWithShadow(Content, ElementSchema.Font, ElementSchema.Color, ElementSchema.ShadowColor, ElementSchema.ShadowSize, ElementSchema.Area, ElementSchema.MaxSize, ElementSchema.StringFormat, ElementSchema.MinSize, true, ElementSchema.Wrap);
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
                ElementSchema.MaxSize, ElementSchema.StringFormat, ElementSchema.Wrap, ElementSchema.StretchImage, ElementSchema.JoinDirection);
        }
    }
}

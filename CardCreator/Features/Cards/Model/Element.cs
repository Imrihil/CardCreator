using CardCreator.Features.Drawing;
using CardCreator.Features.Images;
using MyWarCreator.Extensions;
using System.Drawing;

namespace CardCreator.Features.Cards.Model
{
    public class Element : IDrawable
    {
        public string Content { get; }
        public ElementSchema ElementSchema { get; }
        private Image Image { get; }

        public Element(IImageProvider imageProvider, string content, ElementSchema elementSchema)
        {
            Content = content;
            Image = imageProvider.TryGet(content);
            ElementSchema = elementSchema;
        }

        public void Draw(Graphics graphics)
        {
            if(ElementSchema.Background != null)
                graphics.DrawImage(ElementSchema.Background, ElementSchema.Area);

            if (Image != null)
                graphics.DrawImage(Image, ElementSchema.Area);
            else
                graphics.DrawAdjustedString(Content, ElementSchema.Font, Color.Black, ElementSchema.Area, ElementSchema.MaxSize, ElementSchema.StringFormat, ElementSchema.MinSize);
        }
    }
}

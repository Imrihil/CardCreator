using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CardCreator.Features.Drawing;
using CardCreator.Features.Images;

namespace CardCreator.Features.Cards.Model
{
    public class Card : List<Element>, IDrawable
    {
        public CardSchema CardSchema { get; }
        public string Name => this.FirstOrDefault(element => IsNameElement(element))?.Content;
        private Image Image { get; set; }

        public Card(IImageProvider imageProvider, CardSchema cardSchema, IEnumerable<string> cardElements) : base(new List<Element>())
        {
            CardSchema = cardSchema;

            foreach (var element in cardSchema.Zip(cardElements, (elementSchema, content) => new Element(imageProvider, content, elementSchema)))
            {
                Add(element);
            }
        }

        public void Draw(Graphics graphics)
        {
            if (CardSchema.Background != null)
                graphics.DrawImage(CardSchema.Background, new Rectangle(0, 0, CardSchema.WidthPx, CardSchema.HeightPx));

            ForEach(element => element.Draw(graphics));
        }

        public Image GetImage()
        {
            if (Image != null) return Image;

            Image = new Bitmap(CardSchema.WidthPx, CardSchema.HeightPx);

            using var graphics = Graphics.FromImage(Image);
            graphics.FillRectangle(Brushes.White, 0, 0, CardSchema.WidthPx, CardSchema.HeightPx);
            graphics.DrawRectangle(Pens.Black, 0, 0, CardSchema.WidthPx - 2, CardSchema.HeightPx - 2);
            Draw(graphics);

            return Image;
        }

        private bool IsNameElement(Element element)
        {
            var name = element.ElementSchema.Name.ToLower();
            return name == "name" || name == "nazwa";
        }
    }
}

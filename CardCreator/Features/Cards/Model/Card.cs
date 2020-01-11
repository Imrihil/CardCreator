using System;
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
        public int Repetitions { get; set; }
        private Image Image { get; set; }

        public Card(IImageProvider imageProvider, CardSchema cardSchema, IEnumerable<string> cardElements, string directory, bool generateImages = true) :
            base(InitElements(imageProvider, cardSchema, cardElements, directory, generateImages))
        {
            CardSchema = cardSchema;

            MergeElementsByName();
        }

        private static IEnumerable<Element> InitElements(IImageProvider imageProvider, CardSchema cardSchema, IEnumerable<string> cardElements, string directory, bool generateImages)
        {
            var i = 0;
            var elements = cardElements
                .Where(_ => !cardSchema.CommentIdxs.Contains(i++))
                .Zip(cardSchema, (content, elementSchema) =>
                       elementSchema.Background == null && string.IsNullOrEmpty(content) ? null :
                           new Element(imageProvider, content, elementSchema, directory, generateImages))
                       .Where(element => element != null);

            return elements;
        }

        private void MergeElementsByName()
        {
            foreach (var elementsGroup in this.GroupBy(element => element.ElementSchema.Name))
            {
                var all = elementsGroup.Count();
                if (all == 1) continue;

                var position = 0;
                foreach (var element in elementsGroup)
                {
                    element.SetPosition(position++, all);
                }
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
            graphics.DrawRectangle(Pens.Black, 0, 0, CardSchema.WidthPx - 1, CardSchema.HeightPx - 1);
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

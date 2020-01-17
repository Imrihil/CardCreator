using CardCreator.Features.Drawing;
using MediatR;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CardCreator.Features.Cards.Model
{
    public sealed class Card : List<Element>, IDrawable, IDisposable
    {
        public CardSchema CardSchema { get; }
        public string Name => this.FirstOrDefault(element => IsNameElement(element))?.Content;
        public int Repetitions { get; set; }

        private Image image;
        public Image Image
        {
            get
            {
                if (image != null) return image;

                image = new Bitmap(CardSchema.WidthPx, CardSchema.HeightPx);

                using var graphics = Graphics.FromImage(image);
                graphics.FillRectangle(Brushes.White, 0, 0, CardSchema.WidthPx, CardSchema.HeightPx);
                graphics.DrawRectangle(Pens.Black, 0, 0, CardSchema.WidthPx - 1, CardSchema.HeightPx - 1);
                Draw(graphics);

                return image;
            }
        }

        private bool disposed = false;

        public Card(IMediator mediator, IImageProvider imageProvider, CardSchema cardSchema, IEnumerable<string> cardElements, string directory, bool generateImages = true) :
            base(InitElements(mediator, imageProvider, cardSchema, cardElements, directory, generateImages))
        {
            CardSchema = cardSchema;

            MergeElementsByName();
        }

        private static IEnumerable<Element> InitElements(IMediator mediator, IImageProvider imageProvider, CardSchema cardSchema, IEnumerable<string> cardElements, string directory, bool generateImages)
        {
            var i = 0;
            var elements = cardElements
                .Where(_ => !cardSchema.CommentIdxs.Contains(i++))
                .Zip(cardSchema, (content, elementSchema) =>
                       elementSchema.Background == null && string.IsNullOrEmpty(content) ? null :
                           new Element(mediator, imageProvider, content, elementSchema, directory, generateImages))
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
            {
                using var background = CardSchema.Background.GetNewBitmap();
                graphics.DrawImage(background, new Rectangle(0, 0, CardSchema.WidthPx, CardSchema.HeightPx));
            }

            ForEach(element => element.Draw(graphics));
        }

        private bool IsNameElement(Element element)
        {
            var name = element.ElementSchema.Name.ToLower();
            return name == "name" || name == "nazwa";
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
                image?.Dispose();
                foreach (var element in this)
                    element.Dispose();
            }

            disposed = true;
        }
    }
}

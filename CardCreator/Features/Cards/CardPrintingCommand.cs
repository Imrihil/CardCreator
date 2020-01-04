using CardCreator.Features.Cards.Model;
using MediatR;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CardCreator.Features.Cards
{
    public class CardPrintingCommand : IRequest<bool>
    {
        public Card Card { get; }
        public string DirectoryPath { get; }
        public int? Number { get; }

        public CardPrintingCommand(Card card, string directoryPath, int? number = null)
        {
            Card = card;
            DirectoryPath = directoryPath;
            Number = number;
        }
    }

    public class CardPrintingHandler : IRequestHandler<CardPrintingCommand, bool>
    {
        private const string cards = "cards";

        public async Task<bool> Handle(CardPrintingCommand request, CancellationToken cancellationToken)
        {
            var width = request.Card.CardSchema.WidthPx;
            var height = request.Card.CardSchema.HeightPx;

            using var bitmap = new Bitmap(width, height);
            using var graphics = Graphics.FromImage(bitmap);
            graphics.FillRectangle(Brushes.White, 0, 0, width, height);
            graphics.DrawRectangle(Pens.Black, 0, 0, width - 2, height - 2);
            request.Card.Draw(graphics);
            bitmap.Save(Path.Combine(request.DirectoryPath, cards, GetFileName(request)), ImageFormat.Png);

            return await Task.FromResult(true);
        }

        private string GetFileName(CardPrintingCommand request)
            => (request.Card.Name ?? request.Number?.ToString() ?? "card") + ".png";

    }
}

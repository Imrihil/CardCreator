using CardCreator.Features.Cards.Model;
using CardCreator.Features.Fonts;
using CardCreator.Features.Images;
using CardCreator.Features.System;
using CardCreator.Settings;
using CardCreator.View;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CardCreator.Features.Cards
{
    public class CardGeneratingCommand : IRequest<int>
    {
        public string FilePath { get; }
        public CancellationTokenSource Cts { get; }

        public CardGeneratingCommand(string filePath, CancellationTokenSource cts)
        {
            FilePath = filePath;
            Cts = cts;
        }
    }

    public class CardGeneratingHandler : CardGeneratingBaseHandler, IRequestHandler<CardGeneratingCommand, int>
    {
        private readonly string directoryName;

        public CardGeneratingHandler(IOptions<AppSettings> settings, IMediator mediator, IFontProvider fontProvider, IImageProvider imageProvider, ProcessWindow processWindow) : base(mediator, fontProvider, imageProvider, processWindow)
        {
            directoryName = settings.Value.CardsDirectory;
        }

        public async Task<int> Handle(CardGeneratingCommand request, CancellationToken cancellationToken)
        {
            ProcessWindow.RegisterCancelationToken(request.Cts);
            ProcessWindow.Show();

            var file = new FileInfo(request.FilePath);
            if (!file.Exists)
            {
                ProcessWindow.LogMessage($"File {file.Name} not exists, so action cannot be processed.");
                return 0;
            }

            ProcessWindow.LogMessage($"Reading {file.Name} ...");
            var readCardFile = await ReadCardFile(file);
            if (readCardFile == null) return 0;
            ProcessWindow.LogMessage($"... done.");
            ProcessWindow.SetProgress(100.0 / (1.0 + Math.Max(CardSchema.ParamsNumber, ElementSchema.ParamsNumber) + readCardFile.CardsElements.Count));

            ProcessWindow.LogMessage($"Initializing card schemas ...");
            var cardSchema = await GetCardSchema(readCardFile, file.DirectoryName);
            if (cardSchema == null) return 0;
            ProcessWindow.LogMessage($"... done.");
            ProcessWindow.SetProgress(GetProgress(0, readCardFile.CardsElements.Count));

            ProcessWindow.LogMessage($"Generating cards ...");
            var successes = await GenerateCards(readCardFile, cardSchema, file);
            ProcessWindow.LogMessage($"... done.");

            return successes;
        }

        private async Task<int> GenerateCards(ReadCardFileResults readCardFile, CardSchema cardSchema, FileInfo file)
        {
            var directory = Path.Combine(file.DirectoryName, directoryName);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var i = 0;
            var successes = 0;
            foreach (var cardElements in readCardFile.CardsElements)
            {
                if (readCardFile.CardsRepetitions[i] > 0)
                {
                    try
                    {
                        var card = new Card(ImageProvider, cardSchema, cardElements, file.DirectoryName);
                        try
                        {
                            card.GetImage().Save(Path.Combine(directory, GetFileName(card, i)), ImageFormat.Png);
                            ++successes;
                            ProcessWindow.LogMessage($"{(i + 1).ToOrdinal()} card saved: {card.Name}.");
                        }
                        catch (Exception ex)
                        {
                            ProcessWindow.LogMessage($"An error occured while processing {(i + 1).ToOrdinal()} card {card.Name}: {ex}");
                        }
                    }
                    catch (Exception ex)
                    {
                        ProcessWindow.LogMessage($"An error occured while processing {(i + 1).ToOrdinal()} card: {ex}");
                    }
                }
                ++i;
                ProcessWindow.SetProgress(GetProgress(i, readCardFile.CardsElements.Count));
            }
            return await Task.FromResult(successes);
        }

        private string GetFileName(Card card, int? number)
            => $"{card.Name ?? number?.ToString() ?? "card"}.png";
    }
}

using CardCreator.Features.Cards.Model;
using CardCreator.Features.Fonts;
using CardCreator.Features.Drawing;
using CardCreator.Features.System;
using CardCreator.Features.Threading;
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
    public class CardGeneratingCommand : IRequest<bool>
    {
        public string FilePath { get; }
        public bool GenerateImages { get; set; }
        public CancellationTokenSource Cts { get; }

        public CardGeneratingCommand(string filePath, bool generateImages, CancellationTokenSource cts)
        {
            FilePath = filePath;
            GenerateImages = generateImages;
            Cts = cts;
        }
    }

    public class CardGeneratingHandler : CardGeneratingBaseHandler, IRequestHandler<CardGeneratingCommand, bool>
    {
        private const string titleResourceString = "GeneratingCards";
        private readonly string directoryName;

        public CardGeneratingHandler(IOptions<AppSettings> settings, IMediator mediator, IFontProvider fontProvider, IImageProvider imageProvider, ProcessWindow processWindow) : base(mediator, fontProvider, imageProvider, processWindow)
        {
            directoryName = settings.Value.CardsDirectory;
        }

        public async Task<bool> Handle(CardGeneratingCommand request, CancellationToken cancellationToken)
        {
            ProcessWindow.RegisterCancelationToken(request.Cts);
            ProcessWindow.Show();
            ProcessWindow.Title = $"{Properties.Resources.ResourceManager.GetString(titleResourceString)} {request.FilePath}";
            ThreadManager.RunActionInNewThread(async () =>
            {
                var file = new FileInfo(request.FilePath);
                if (!file.Exists)
                {
                    ProcessWindow.LogMessage($"File {file.Name} not exists, so action cannot be processed.");
                    return;
                }

                ProcessWindow.LogMessage($"Reading {file.Name} ...");
                var readCardFile = await ReadCardFile(file);
                if (readCardFile == null) return;
                ProcessWindow.LogMessage($"... done.");
                ProcessWindow.SetProgress(100.0 / (1.0 + Math.Max(CardSchema.ParamsNumber, ElementSchema.ParamsNumber) + readCardFile.CardsElements.Count));

                ProcessWindow.LogMessage($"Initializing card schemas ...");
                using var cardSchema = await GetCardSchema(readCardFile, file.DirectoryName, request.GenerateImages);
                if (cardSchema == null) return;
                ProcessWindow.LogMessage($"... done.");
                ProcessWindow.SetProgress(GetProgress(0, readCardFile.CardsElements.Count));

                ProcessWindow.LogMessage($"Generating cards ...");
                var successes = await GenerateCards(readCardFile, cardSchema, file, request.GenerateImages);
                ProcessWindow.LogMessage($"... done.");
            });

            return await Task.FromResult(true);
        }

        private async Task<int> GenerateCards(ReadCardFileResults readCardFile, CardSchema cardSchema, FileInfo file, bool generateImages)
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
                        using var card = new Card(ImageProvider, cardSchema, cardElements, file.DirectoryName, generateImages);
                        try
                        {
                            using var cardImage = card.Image;
                            cardImage.Save(Path.Combine(directory, GetFileName(card, i)), ImageFormat.Png);
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

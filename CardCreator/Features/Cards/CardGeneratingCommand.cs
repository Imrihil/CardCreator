using CardCreator.Features.Cards.Model;
using CardCreator.Features.Fonts;
using CardCreator.Features.Images;
using CardCreator.View;
using MediatR;
using System;
using System.IO;
using System.Linq;
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

    public class CardGeneratingHandler : IRequestHandler<CardGeneratingCommand, int>
    {
        private readonly IMediator mediator;
        private readonly IImageProvider imageProvider;
        private readonly IFontProvider fontProvider;
        private readonly ProcessWindow processWindow;

        public CardGeneratingHandler(IMediator mediator, IFontProvider fontProvider, IImageProvider imageProvider, ProcessWindow processWindow)
        {
            this.mediator = mediator;
            this.fontProvider = fontProvider;
            this.imageProvider = imageProvider;
            this.processWindow = processWindow;
        }

        public async Task<int> Handle(CardGeneratingCommand request, CancellationToken cancellationToken)
        {
            processWindow.RegisterCancelationToken(request.Cts);
            processWindow.Show();

            var file = new FileInfo(request.FilePath);
            if (!file.Exists)
            {
                processWindow.LogMessage($"File {file.Name} not exists, so action cannot be processed.");
            }

            processWindow.LogMessage($"Reading {file.Name} ...");
            ReadCardFileResults readCardFile;
            try
            {
                readCardFile = await mediator.Send(new ReadCardFileCommand(file));
            }
            catch (Exception ex)
            {
                processWindow.LogMessage(ex);
                return 0;
            }
            processWindow.LogMessage($"... done.");
            processWindow.SetProgress(100.0 / (1.0 + Math.Max(CardSchema.ParamsNumber, ElementSchema.ParamsNumber) + readCardFile.CardsElements.Count));

            processWindow.LogMessage($"Initializing card schemas ...");
            CardSchema cardSchema;
            try
            {
                cardSchema = new CardSchema(processWindow, fontProvider, imageProvider, readCardFile.CardSchemaParams, readCardFile.ElementSchemasParams);
            }
            catch (ArgumentException ex)
            {
                processWindow.LogMessage(ex.Message);
                return 0;
            }
            catch (Exception ex)
            {
                processWindow.LogMessage(ex);
                return 0;
            }
            processWindow.LogMessage($"... done.");
            processWindow.SetProgress(GetProgress(0, readCardFile.CardsElements.Count));

            var i = 0;
            var successes = 0;
            foreach (var cardElements in readCardFile.CardsElements)
            {
                try
                {
                    ++i;
                    var card = new Card(imageProvider, cardSchema, cardElements);
                    try
                    {
                        await mediator.Send(new CardPrintingCommand(card, file.Directory.FullName, i));
                        processWindow.SetProgress(GetProgress(i, readCardFile.CardsElements.Count));
                        ++successes;
                    }
                    catch (Exception ex)
                    {
                        processWindow.LogMessage($"An error occured while processing {i}th card {card.Name}: {ex}");
                    }
                }
                catch (Exception ex)
                {
                    processWindow.LogMessage($"An error occured while processing {i}th card: {ex}");
                }
            }

            return successes;
        }

        private double GetProgress(int generatedCardsNumber, int allCardsNumber) =>
            100.0 * (1.0 + Math.Max(CardSchema.ParamsNumber, ElementSchema.ParamsNumber) + generatedCardsNumber) / (1.0 + Math.Max(CardSchema.ParamsNumber, ElementSchema.ParamsNumber) + allCardsNumber);
    }
}

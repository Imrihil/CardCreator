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
    public class CardGeneratingCommand : IRequest<bool>
    {
        public string FilePath { get; }
        public CancellationTokenSource Cts { get; }

        public CardGeneratingCommand(string filePath, CancellationTokenSource cts)
        {
            FilePath = filePath;
            Cts = cts;
        }
    }

    public class CardGeneratingHandler : IRequestHandler<CardGeneratingCommand, bool>
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

        public async Task<bool> Handle(CardGeneratingCommand request, CancellationToken cancellationToken)
        {
            processWindow.RegisterCancelationToken(request.Cts);
            processWindow.Show();

            if (!File.Exists(request.FilePath))
            {
                processWindow.LogMessage($"File {request.FilePath} not exists, so action cannot be processed.");
            }

            processWindow.LogMessage($"Reading {request.FilePath} ...");
            ReadCardFileResults readCardFile;
            try
            {
                readCardFile = await mediator.Send(new ReadCardFileCommand(request.FilePath));
            }
            catch (Exception ex)
            {
                processWindow.LogMessage(ex);
                return false;
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
                return false;
            }
            catch (Exception ex)
            {
                processWindow.LogMessage(ex);
                return false;
            }
            processWindow.LogMessage($"... done.");
            processWindow.SetProgress(GetProgress(0, readCardFile.CardsElements.Count));

            var cards = readCardFile.CardsElements.Select(cardElements => new Card(imageProvider, cardSchema, cardElements));



            return await Task.FromResult(false);
        }

        private double GetProgress(int generatedCardsNumber, int allCardsNumber) =>
            100.0 * (1.0 + Math.Max(CardSchema.ParamsNumber, ElementSchema.ParamsNumber) + generatedCardsNumber) / (1.0 + Math.Max(CardSchema.ParamsNumber, ElementSchema.ParamsNumber) + allCardsNumber);
    }
}

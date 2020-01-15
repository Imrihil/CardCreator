using CardCreator.Features.Cards.Model;
using CardCreator.Features.Fonts;
using CardCreator.Features.Drawing;
using CardCreator.View;
using MediatR;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CardCreator.Features.Cards
{
    public abstract class CardGeneratingBaseHandler
    {
        protected readonly IMediator Mediator;
        protected readonly IImageProvider ImageProvider;
        protected readonly IFontProvider FontProvider;
        protected readonly ProcessWindow ProcessWindow;

        public CardGeneratingBaseHandler(IMediator mediator, IFontProvider fontProvider, IImageProvider imageProvider, ProcessWindow processWindow)
        {
            Mediator = mediator;
            FontProvider = fontProvider;
            ImageProvider = imageProvider;
            ProcessWindow = processWindow;
        }

        protected async Task<ReadCardFileResults> ReadCardFile(FileInfo file)
        {
            try
            {
                return await Mediator.Send(new ReadCardFileCommand(file));
            }
            catch (Exception ex)
            {
                ProcessWindow.LogMessage(ex);
                return null;
            }
        }

        protected async Task<CardSchema> GetCardSchema(ReadCardFileResults readCardFile, string directory, bool generateImages)
        {
            try
            {
                return await Task.FromResult(new CardSchema(ProcessWindow, FontProvider, ImageProvider, readCardFile.CardSchemaParams, readCardFile.ElementSchemasParams, directory, generateImages));
            }
            catch (ArgumentException ex)
            {
                ProcessWindow.LogMessage(ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                ProcessWindow.LogMessage(ex);
                return null;
            }
        }

        protected static double GetProgress(int generatedCardsNumber, int allCardsNumber) =>
            100.0 * (1.0 + Math.Max(CardSchema.ParamsNumber, ElementSchema.ParamsNumber) + generatedCardsNumber) / (1.0 + Math.Max(CardSchema.ParamsNumber, ElementSchema.ParamsNumber) + allCardsNumber);
    }
}

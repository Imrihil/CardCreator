using CardCreator.Features.Cards.Model;
using CardCreator.Features.Fonts;
using CardCreator.Features.Drawing;
using CardCreator.View;
using MediatR;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using CardCreator.Settings;

namespace CardCreator.Features.Cards
{
    public abstract class CardGeneratingBaseHandler
    {
        protected readonly TextSettings TextSettings;
        protected readonly IMediator Mediator;
        protected readonly IImageProvider ImageProvider;
        protected readonly IIconProvider IconProvider;
        protected readonly IFontProvider FontProvider;
        protected readonly ProcessWindow ProcessWindow;

        protected CardGeneratingBaseHandler(IOptions<AppSettings> settings, IMediator mediator, IFontProvider fontProvider, IImageProvider imageProvider, IIconProvider iconProvider, ProcessWindow processWindow)
        {
            TextSettings = settings.Value.Text;
            Mediator = mediator;
            FontProvider = fontProvider;
            ImageProvider = imageProvider;
            IconProvider = iconProvider;
            ProcessWindow = processWindow;
        }

        protected async Task<ReadCardFileResults> ReadCardFile(FileInfo file)
        {
            try
            {
                return await Mediator.Send(new ReadCardFileCommand(ProcessWindow, file));
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

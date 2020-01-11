﻿using CardCreator.Features.Cards.Model;
using CardCreator.Features.Fonts;
using CardCreator.Features.Images;
using CardCreator.Features.System;
using CardCreator.Settings;
using CardCreator.View;
using MediatR;
using Microsoft.Extensions.Options;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CardCreator.Features.Cards
{
    public class PdfGeneratingCommand : IRequest<int>
    {
        public string FilePath { get; set; }
        public bool GenerateImages { get; set; }
        public CancellationTokenSource Cts { get; set; }

        public PdfGeneratingCommand(string filePath, bool generateImages, CancellationTokenSource cts)
        {
            FilePath = filePath;
            GenerateImages = generateImages;
            Cts = cts;
        }
    }

    public class PdfGeneratingHandler : CardGeneratingBaseHandler, IRequestHandler<PdfGeneratingCommand, int>
    {
        private readonly PageSize PageSize;
        private readonly PageOrientation PageOrientation;
        private readonly double PageWidthInch;
        private readonly double PageHeightInch;
        private readonly double PageMarginPts;
        private readonly double CardsMarginPts;

        public PdfGeneratingHandler(IOptions<AppSettings> settings, IMediator mediator, IFontProvider fontProvider, IImageProvider imageProvider, ProcessWindow processWindow) :
            base(mediator, fontProvider, imageProvider, processWindow)
        {
            PageSize = settings.Value.PageSize;
            PageOrientation = settings.Value.PageOrientation;
            PageWidthInch = settings.Value.PageWidthInch;
            PageHeightInch = settings.Value.PageHeightInch;
            PageMarginPts = settings.Value.PageMarginPts;
            CardsMarginPts = settings.Value.CardsMarginPts;
        }

        public async Task<int> Handle(PdfGeneratingCommand request, CancellationToken cancellationToken)
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
            var cardSchema = await GetCardSchema(readCardFile, file.DirectoryName, request.GenerateImages);
            if (cardSchema == null) return 0;
            ProcessWindow.LogMessage($"... done.");
            ProcessWindow.SetProgress(GetProgress(0, readCardFile.CardsElements.Count));

            ProcessWindow.LogMessage("Creating document ...");
            var successes = await GeneratePdf(readCardFile, cardSchema, file, request.GenerateImages);
            ProcessWindow.LogMessage($"... done.");

            return successes;
        }

        private async Task<int> GeneratePdf(ReadCardFileResults readCardFile, CardSchema cardSchema, FileInfo file, bool generateImages)
        {
            using var pdf = new PdfDocument();
            PdfPage pdfPage = null;

            var cardWidth = cardSchema.WidthInch * AppSettings.PointsInInch;
            var cardHeight = cardSchema.HeightInch * AppSettings.PointsInInch;
            var cardsInRow = Math.Max(1, (int)((PageWidthInch * AppSettings.PointsInInch - 2 * PageMarginPts) / (cardWidth + CardsMarginPts)));
            var cardsInCol = Math.Max(1, (int)((PageHeightInch * AppSettings.PointsInInch - 2 * PageMarginPts) / (cardHeight + CardsMarginPts)));
            var cardsPerPage = cardsInRow * cardsInCol;

            var i = 0;
            var successes = 0;
            var nCard = 0;
            foreach (var cardElements in readCardFile.CardsElements)
            {
                try
                {
                    var card = new Card(ImageProvider, cardSchema, cardElements, file.DirectoryName, generateImages);
                    try
                    {
                        var n = readCardFile.CardsRepetitions[i];
                        if (n > 0)
                        {
                            using var xImage = XImage.FromGdiPlusImage(card.GetImage());
                            for (var j = 0; j < n; ++j)
                            {
                                if (nCard % cardsPerPage == 0)
                                {
                                    pdfPage = pdf.AddPage();
                                    pdfPage.Size = PageSize;
                                    pdfPage.Orientation = PageOrientation;
                                }
                                using var xGraphics = XGraphics.FromPdfPage(pdfPage);
                                xGraphics.DrawImage(
                                    xImage,
                                    nCard % cardsInRow * (cardWidth + CardsMarginPts) + PageMarginPts,
                                    nCard % cardsPerPage / cardsInRow * (cardHeight + CardsMarginPts) + PageMarginPts,
                                    cardWidth,
                                    cardHeight);
                                ++nCard;
                            }
                            ++successes;
                            ProcessWindow.LogMessage($"{(i + 1).ToOrdinal()} card added to file {n} times: {card.Name}.");
                        }
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
                ++i;
                ProcessWindow.SetProgress(GetProgress(i, readCardFile.CardsElements.Count + 1));
            }
            try
            {
                var fileName = $"{file.Name.Substring(0, file.Name.LastIndexOf("."))}.pdf";
                pdf.Save(Path.Combine(file.DirectoryName, fileName));
                ProcessWindow.SetProgress(GetProgress(i + 1, readCardFile.CardsElements.Count + 1));
                ProcessWindow.LogMessage($"The document {fileName} saved.");
            }
            catch (Exception ex)
            {
                ProcessWindow.LogMessage($"An error occured while saving document: {ex}");
                return 0;
            }

            return await Task.FromResult(successes);
        }
    }
}
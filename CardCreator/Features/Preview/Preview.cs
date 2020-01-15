using CardCreator.Features.Cards;
using CardCreator.Features.Cards.Model;
using CardCreator.Features.Drawing;
using CardCreator.Features.Fonts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CardCreator.Features.Preview
{
    public class Preview : IPreview, IDisposable
    {
        private FileInfo File { get; }

        private int CurrentPosition { get; set; }
        private int MaxPosition => CardsElements?.Count ?? 0;
        public bool GenerateImages { get; private set; }

        private CardSchema CardSchema { get; set; }
        private List<List<string>> CardsElements { get; set; }
        private Dictionary<int, Image> CardImages { get; }
        private BitmapImage LastBitmapImage { get; set; }

        private readonly IMediator mediator;
        private readonly IFontProvider fontProvider;
        private readonly IImageProvider imageProvider;

        private readonly Color gridColor;
        private readonly Font gridFont;

        private bool disposed = false;

        public Preview(IMediator mediator, IFontProvider fontProvider, IImageProvider imageProvider, string filePath, bool generateImages)
        {
            this.mediator = mediator;
            this.fontProvider = fontProvider;
            this.imageProvider = imageProvider;

            gridColor = Color.FromArgb(128, 255, 0, 0);
            gridFont = new Font(fontProvider.TryGet(string.Empty), 12, FontStyle.Bold, GraphicsUnit.Pixel);

            CardImages = new Dictionary<int, Image>();
            File = new FileInfo(filePath);
            GenerateImages = generateImages;
        }

        public async Task<BitmapImage> GetImage(int gridWidth, int gridHeight) =>
            LastBitmapImage ?? DisposeLastBitmapImage((await GetImage(CurrentPosition, gridWidth, gridHeight)).ToBitmapImage());

        public async Task<BitmapImage> Next(int gridWidth, int gridHeight) =>
            DisposeLastBitmapImage((await GetImage((CurrentPosition + 1) % MaxPosition, gridWidth, gridHeight)).ToBitmapImage());

        public async Task<BitmapImage> Previous(int gridWidth, int gridHeight) =>
            DisposeLastBitmapImage((await GetImage(CurrentPosition > 0 ? CurrentPosition - 1 : MaxPosition - 1, gridWidth, gridHeight)).ToBitmapImage());

        private async Task<Image> GetImage(int position, int gridWidth, int gridHeight)
        {
            CurrentPosition = position;
            if (CardSchema == null)
                await Refresh(GenerateImages);

            if (!CardImages.TryGetValue(CurrentPosition, out var cardImage))
            {
                cardImage = new Card(imageProvider, CardSchema, CardsElements[CurrentPosition], File.DirectoryName, GenerateImages).Image;
                CardImages.Add(CurrentPosition, cardImage);
            }

            using var graphics = Graphics.FromImage(cardImage);
            graphics.DrawGrid(gridWidth, gridHeight, cardImage.Width, cardImage.Height, gridColor, gridFont);

            return cardImage;
        }

        public async Task Refresh(bool generateImages)
        {
            GenerateImages = generateImages;
            ClearCache();

            var readCardFile = await mediator.Send(new ReadCardFileCommand(File));

            CardsElements = readCardFile.CardsRepetitions
                .Zip(readCardFile.CardsElements, (cardRepetition, cardElements) => new KeyValuePair<List<string>, int>(cardElements, cardRepetition))
                .Where(kv => kv.Value > 0).Select(kv => kv.Key).ToList();
            CardSchema = new CardSchema(null, fontProvider, imageProvider, readCardFile.CardSchemaParams, readCardFile.ElementSchemasParams, File.DirectoryName, GenerateImages);
        }

        private void ClearCache()
        {
            foreach (var image in CardImages.Values)
                image.Dispose();
            CardImages.Clear();

            LastBitmapImage?.StreamSource.Dispose();
            LastBitmapImage = null;
        }

        private BitmapImage DisposeLastBitmapImage(BitmapImage bitmapImage)
        {
            if (LastBitmapImage != null)
                LastBitmapImage.StreamSource.Dispose();
            LastBitmapImage = bitmapImage;
            return LastBitmapImage;
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
                CardSchema.Dispose();
                LastBitmapImage.StreamSource.Dispose();
                foreach (var image in CardImages.Values)
                    image.Dispose();
            }

            disposed = true;
        }
    }
}

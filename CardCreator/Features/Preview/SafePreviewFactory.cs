using CardCreator.Features.Fonts;
using CardCreator.Features.Images;
using CardCreator.Features.SafeCaller;
using MediatR;
using System;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CardCreator.Features.Preview
{
    public class SafePreviewFactory : PreviewFactory
    {
        public SafePreviewFactory(IMediator mediator, IFontProvider fontProvider, IImageProvider imageProvider) :
            base(mediator, fontProvider, imageProvider)
        { }

        public override async Task<BitmapImage> GetPreviewImage(int gridWidth, int gridHeight) =>
            await Safe<BitmapImage>.CallAsync(async () => await base.GetPreviewImage(gridWidth, gridHeight), new BitmapImage());

        public override async Task<BitmapImage> NextPreviewImage(int gridWidth, int gridHeight) =>
            await Safe<BitmapImage>.CallAsync(async () => await base.NextPreviewImage(gridWidth, gridHeight), new BitmapImage());

        public override async Task<BitmapImage> PreviousPreviewImage(int gridWidth, int gridHeight) =>
            await Safe<BitmapImage>.CallAsync(async () => await base.PreviousPreviewImage(gridWidth, gridHeight), new BitmapImage());

        public override async Task Refresh(bool generateImages) =>
            await Safe.CallAsync(async () => await base.Refresh(generateImages));

        public override async Task<string> Register(string filePath, bool generateImages) =>
            await Safe<string>.CallAsync(async () => await base.Register(filePath, generateImages), string.Empty);

        public override async Task<bool> Register(string key, string filePath, bool generateImages) =>
            await Safe<bool>.CallAsync(async () => await base.Register(key, filePath, generateImages), false);

        public override async Task SetCurrentPreview(string key, bool generateImages) =>
            await Safe.CallAsync(async () => await base.SetCurrentPreview(key, generateImages));
    }
}

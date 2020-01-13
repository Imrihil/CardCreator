using CardCreator.Features.Fonts;
using CardCreator.Features.Images;
using MediatR;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CardCreator.Features.Preview
{
    public class PreviewFactory : IPreviewFactory
    {
        private readonly Dictionary<string, IPreview> previews;
        private IPreview CurrentPreview { get; set; }

        private readonly IMediator mediator;
        private readonly IFontProvider fontProvider;
        private readonly IImageProvider imageProvider;

        public PreviewFactory(IMediator mediator, IFontProvider fontProvider, IImageProvider imageProvider)
        {
            this.mediator = mediator;
            this.fontProvider = fontProvider;
            this.imageProvider = imageProvider;

            previews = new Dictionary<string, IPreview>();
        }

        public async Task<BitmapImage> GetPreviewImage() =>
            await CurrentPreview.GetImage();

        public async Task<BitmapImage> NextPreviewImage() =>
            await CurrentPreview.Next();

        public async Task<BitmapImage> PreviousPreviewImage() =>
            await CurrentPreview.Previous();

        public string Register(string filePath, bool generateImages)
        {
            var key = previews.Count.ToString();
            previews.Add(key, new Preview(mediator, fontProvider, imageProvider, filePath, generateImages));
            return key;
        }

        public bool Register(string key, string filePath, bool generateImages)
        {
            if (previews.ContainsKey(key))
            {
                previews[key] = new Preview(mediator, fontProvider, imageProvider, filePath, generateImages);
                return true;
            }
            previews.Add(key, new Preview(mediator, fontProvider, imageProvider, filePath, generateImages));
            return false;
        }

        public async Task SetCurrentPreview(string key, bool generateImages)
        {
            CurrentPreview = previews[key];

            if (CurrentPreview.GenerateImages != generateImages)
                await Refresh(generateImages);
        }

        public async Task Refresh(bool generateImages) =>
            await CurrentPreview.Refresh(generateImages);
    }
}

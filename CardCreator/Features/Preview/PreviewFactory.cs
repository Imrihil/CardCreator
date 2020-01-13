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

        public virtual async Task<BitmapImage> GetPreviewImage() =>
            await CurrentPreview.GetImage();

        public virtual async Task<BitmapImage> NextPreviewImage() =>
            await CurrentPreview.Next();

        public virtual async Task<BitmapImage> PreviousPreviewImage() =>
            await CurrentPreview.Previous();

        public virtual async Task<string> Register(string filePath, bool generateImages)
        {
            var key = previews.Count.ToString();
            previews.Add(key, new Preview(mediator, fontProvider, imageProvider, filePath, generateImages));
            return await Task.FromResult(key);
        }

        public virtual async Task Refresh(bool generateImages) =>
            await CurrentPreview.Refresh(generateImages);

        public virtual async Task<bool> Register(string key, string filePath, bool generateImages)
        {
            if (previews.ContainsKey(key))
            {
                previews[key] = new Preview(mediator, fontProvider, imageProvider, filePath, generateImages);
                return true;
            }
            previews.Add(key, new Preview(mediator, fontProvider, imageProvider, filePath, generateImages));
            return await Task.FromResult(false);
        }

        public virtual async Task SetCurrentPreview(string key, bool generateImages)
        {
            CurrentPreview = previews[key];

            if (CurrentPreview.GenerateImages != generateImages)
                await Refresh(generateImages);
        }
    }
}

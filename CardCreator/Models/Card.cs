using System.Drawing;
using CardCreator.Features.Drawing;
using CardCreator.Features.Fonts;
using CardCreator.Features.Images;

namespace CardCreator.Models
{
    public class Card
    {
        private readonly IFontProvider fontProvider;
        private readonly IPainter painter;
        private readonly IImageProvider imageProvider;

        protected Card(IFontProvider fontProvider, IPainter painter, IImageProvider imageProvider, string dirPath)
        {
            this.fontProvider = fontProvider;
            this.painter = painter;
            this.imageProvider = imageProvider;
        }

        protected virtual void DrawCard(Graphics graphics)
        {
        }
    }
}

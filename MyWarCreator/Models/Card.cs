using System.Drawing;
using MyWarCreator.Features.Drawing;
using MyWarCreator.Features.Fonts;
using MyWarCreator.Features.Images;

namespace MyWarCreator.Models
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

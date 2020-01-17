using CardCreator.Features.Drawing.Model;
using CardCreator.Features.Drawing.Text.Model;
using MediatR;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace CardCreator.Features.Drawing.Text
{
    public class DrawTextCommand : IRequest<bool>
    {
        public Graphics Graphics { get; }
        public string Content { get; }
        public StringFormatExtended StringFormat { get; }
        public FontFamily FontFamily { get; }
        public int MaxSize { get; }
        public int MinSize { get; }
        public Color Color { get; }
        public Color ShadowColor { get; }
        public int ShadowSize { get; }
        public bool WrapLines { get; }
        public RectangleF LayoutRectangle { get; }

        public DrawTextCommand(Graphics graphics, string content, StringFormatExtended stringFormat, FontFamily fontFamily, int maxSize, int minSize, Color color, Color shadowColor, int shadowSize, bool wrapLines, RectangleF layoutRectangle)
        {
            Graphics = graphics;
            Content = content;
            StringFormat = stringFormat;
            FontFamily = fontFamily;
            MaxSize = maxSize;
            MinSize = minSize;
            Color = color;
            ShadowColor = shadowColor;
            ShadowSize = shadowSize;
            WrapLines = wrapLines;
            LayoutRectangle = layoutRectangle;
        }
    }

    public class DrawTextHandler : IRequestHandler<DrawTextCommand, bool>
    {
        private readonly IIconProvider iconProvider;

        public DrawTextHandler(IIconProvider iconProvider)
        {
            this.iconProvider = iconProvider;
        }

        public async Task<bool> Handle(DrawTextCommand request, CancellationToken cancellationToken)
        {
            using var article = new Article(iconProvider, request.Content);
            article.Draw(request.Graphics, request.StringFormat, request.FontFamily, request.MaxSize, request.MinSize, request.Color, request.ShadowColor, request.ShadowSize, request.WrapLines, request.LayoutRectangle);

            return await Task.FromResult(true);
        }
    }
}

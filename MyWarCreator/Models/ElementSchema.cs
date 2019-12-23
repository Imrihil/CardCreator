using MyWarCreator.Features.Fonts;
using System.Drawing;

namespace MyWarCreator.Models
{
    public class ElementSchema
    {
        public string Name { get; }
        public Rectangle Area { get; }
        public FontFamily Font { get; }
        public int MaxSize { get; }
        public int MinSize { get; }
        public StringFormat StringFormat { get; }

        public ElementSchema(IFontProvider fontProvider, string name, int x, int y, int width, int height, string fontName, int maxSize, int minSize, StringFormat stringFormat)
        {
            Name = name;
            Area = new Rectangle(x, y, width, height);
            Font = fontProvider.Get(fontName);
            MaxSize = maxSize;
            MinSize = minSize;
            StringFormat = stringFormat;
        }
    }
}

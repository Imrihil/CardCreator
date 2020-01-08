using CardCreator.Features.Fonts;
using CardCreator.Features.Images;
using CardCreator.Features.Logging;
using CardCreator.Features.System;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace CardCreator.Features.Cards.Model
{
    public class ElementSchema
    {
        public const int ParamsNumber = 11;

        private const int NameIdx = 0;
        private const int BackgroundIdx = 1;
        private const int XIdx = 2;
        private const int YIdx = 3;
        private const int WidthIdx = 4;
        private const int HeightIdx = 5;
        private const int ColorIdx = 6;
        private const int FontIdx = 7;
        private const int MaxSizeIdx = 8;
        private const int StringFormatIdx = 9;
        private const int JoinIdx = 10;
        private readonly string[] ParamsSeparator = new string[] { ",", ", " };

        public Image Background { get; }
        public string Name { get; }
        public Rectangle Area { get; }
        public Color Color { get; }
        public FontFamily Font { get; }
        public int MaxSize { get; }
        public int MinSize { get; }
        public StringFormat StringFormat { get; }

        public ElementSchema(string name, Image background, int x, int y, int width, int height, Color color, FontFamily font, int maxSize, StringFormat stringFormat)
        {
            Name = name;
            Background = background;
            Area = new Rectangle(x, y, width, height);
            Color = color;
            Font = font;
            MaxSize = maxSize;
            MinSize = Math.Min(6, MaxSize);
            StringFormat = stringFormat;
        }

        public ElementSchema(ILogger logger, IImageProvider imageProvider, IFontProvider fontProvider, List<string> parameters, string directory, Color? defaultColor = null) :
            this(
                parameters[NameIdx],
                imageProvider.TryGet(Path.Combine(directory, parameters[BackgroundIdx])),
                Parser<int>.Parse(logger, parameters[XIdx], (param) => int.Parse(param), (val) => val >= 0,
                $"{(XIdx + 1).ToOrdinal()} parameter must be a nonnegative integer, but \"{parameters[XIdx]}\" is not."),
                Parser<int>.Parse(logger, parameters[YIdx], (param) => int.Parse(param), (val) => val >= 0,
                $"{(YIdx + 1).ToOrdinal()} parameter must be a nonnegative integer, but \"{parameters[YIdx]}\" is not."),
                Parser<int>.Parse(logger, parameters[WidthIdx], (param) => int.Parse(param), (val) => val > 0,
                $"{(WidthIdx + 1).ToOrdinal()} parameter must be a positive integer, but \"{parameters[WidthIdx]}\" is not."),
                Parser<int>.Parse(logger, parameters[HeightIdx], (param) => int.Parse(param), (val) => val > 0,
                $"{(HeightIdx + 1).ToOrdinal()} parameter must be a positive integer, but \"{parameters[HeightIdx]}\" is not."),
                TryGetColor(parameters[ColorIdx], defaultColor),
                fontProvider.TryGet(parameters[FontIdx]),
                Parser<int>.Parse(logger, parameters[MaxSizeIdx], (param) => int.Parse(param), (val) => val >= 0,
                $"{(MaxSizeIdx + 1).ToOrdinal()} parameter must be a nonnegative integer, but \"{parameters[MaxSizeIdx]}\" is not."),
                GetStringFormat(parameters[StringFormatIdx])
            )
        { }

        private static Color TryGetColor(string color, Color? @default = null)
        {
            if (string.IsNullOrEmpty(color)) return @default ?? Color.Black;
            try
            {
                return ColorTranslator.FromHtml(color);
            }
            catch
            {
                return Color.Black;
            }
        }

        private static StringFormat GetStringFormat(string param)
        {
            if (string.IsNullOrEmpty(param))
            {
                return FontConsts.LeftFormat;
            }
            return (param.Substring(0, 1).ToUpper()) switch
            {
                "L" => FontConsts.LeftFormat,
                "C" => FontConsts.CenteredFormat,
                "R" => FontConsts.RightFormat,
                _ => throw new ArgumentException($"\"{param}\" is unknown string format. Try: \"L\", \"C\" or \"R\"."),
            };
        }
    }
}

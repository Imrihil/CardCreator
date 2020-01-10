using CardCreator.Features.Fonts;
using CardCreator.Features.Images;
using CardCreator.Features.Logging;
using CardCreator.Features.System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;

namespace CardCreator.Features.Cards.Model
{
    public class CardSchema : List<ElementSchema>
    {
        public const int ParamsNumber = 8;

        private const int NameIdx = 0;
        private const int BackgroundIdx = 1;
        private const int WidthPxIdx = 2;
        private const int HeightPxIdx = 3;
        private const int WidthInchIdx = 4;
        private const int HeightInchIdx = 5;
        private const int ColorIdx = 6;
        private const int ShadowColorIdx = 7;

        public string Name { get; }
        public Image Background { get; }
        public int WidthPx { get; }
        public int HeightPx { get; }
        public double WidthInch { get; }
        public double HeightInch { get; }
        public Color DefaultColor { get; }
        public Color DefaultShadowColor { get; }

        public CardSchema(string name, Image background, int widhtPx, int heightPx, double widthInch, double heightInch, IList<ElementSchema> elementSchemas, Color? defaultColor = null, Color? defaultShadowColor = null) : base(elementSchemas)
        {
            Name = name;
            Background = background;
            WidthPx = widhtPx;
            HeightPx = heightPx;
            WidthInch = widthInch;
            HeightInch = heightInch;
            DefaultColor = defaultColor ?? Color.Black;
            DefaultShadowColor = defaultShadowColor ?? Color.White;
        }

        public CardSchema(ILogger logger, IFontProvider fontProvider, IImageProvider imageProvider, IList<string> parameters, List<List<string>> elementSchemasParams, string directory) :
            this(
                parameters[NameIdx],
                imageProvider.TryGet(Path.Combine(directory, parameters[BackgroundIdx])),
                Parser<int>.Parse(logger, parameters[WidthPxIdx], (param) => int.Parse(param), (val) => val > 0,
                $"{(WidthPxIdx + 1).ToOrdinal()} parameter must be a positive integer, but \"{parameters[WidthPxIdx]}\" is not."),
                Parser<int>.Parse(logger, parameters[HeightPxIdx], (param) => int.Parse(param), (val) => val > 0,
                $"{(HeightPxIdx + 1).ToOrdinal()} parameter must be a positive integer, but \"{parameters[HeightPxIdx]}\" is not."),
                Parser<double>.Parse(logger, parameters[WidthInchIdx], (param) => double.Parse(param.Replace(',', '.'), CultureInfo.InvariantCulture), (val) => val > 0,
                $"{(WidthInchIdx + 1).ToOrdinal()} parameter must be a positive number, but \"{parameters[WidthInchIdx]}\" is not."),
                Parser<double>.Parse(logger, parameters[HeightInchIdx], (param) => double.Parse(param.Replace(',', '.'), CultureInfo.InvariantCulture), (val) => val > 0,
                $"{(HeightInchIdx + 1).ToOrdinal()} parameter must be a positive number, but \"{parameters[HeightInchIdx]}\" is not."),
                InitElementSchemas(logger, fontProvider, imageProvider, elementSchemasParams, directory, TryGetColor(parameters[ColorIdx]), TryGetColor(parameters[ShadowColorIdx])),
                TryGetColor(parameters[ColorIdx]),
                TryGetColor(parameters[ShadowColorIdx])
            )
        { }

        private static IList<ElementSchema> InitElementSchemas(ILogger logger, IFontProvider fontProvider, IImageProvider imageProvider, List<List<string>> elementSchemasParams, string directory, Color? defaultColor, Color? defaultShadowColor)
        {
            var elementSchemas = new List<ElementSchema>(elementSchemasParams.Select(elementSchemaParams => new ElementSchema(logger, imageProvider, fontProvider, elementSchemaParams, directory, defaultColor ?? Color.Black, defaultShadowColor ?? Color.White)));

            return elementSchemas;
        }

        private static Color? TryGetColor(string color)
        {
            if (string.IsNullOrEmpty(color)) return null;
            try
            {
                return ColorTranslator.FromHtml(color);
            }
            catch
            {
                return null;
            }
        }
    }
}

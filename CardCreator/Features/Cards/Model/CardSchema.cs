using CardCreator.Features.Fonts;
using CardCreator.Features.Drawing;
using CardCreator.Features.Logging;
using CardCreator.Features.System;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CardCreator.Features.Cards.Model
{
    public sealed class CardSchema : List<ElementSchema>, IDisposable
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
        public ISet<int> CommentIdxs { get; }

        private bool disposed = false;

        public CardSchema(string name, Image background, int widhtPx, int heightPx, double widthInch, double heightInch, IList<ElementSchema> elementSchemas, Color? defaultColor = null, Color? defaultShadowColor = null, ISet<int> commentIdx = null) : base(elementSchemas)
        {
            Name = name;
            Background = background;
            WidthPx = widhtPx;
            HeightPx = heightPx;
            WidthInch = widthInch;
            HeightInch = heightInch;
            DefaultColor = defaultColor ?? Color.Black;
            DefaultShadowColor = defaultShadowColor ?? Color.White;
            CommentIdxs = commentIdx ?? new HashSet<int>();
        }

        public CardSchema(ILogger logger, IFontProvider fontProvider, IImageProvider imageProvider, IList<string> parameters, List<List<string>> elementSchemasParams, string directory, bool generateImages = true) :
            this(
                parameters[NameIdx],
                generateImages ? imageProvider.TryGet(Path.Combine(directory, parameters[BackgroundIdx])) ?? imageProvider.TryGetImageFromColor(parameters[BackgroundIdx], parameters[WidthPxIdx], parameters[HeightPxIdx]) : null,
                Parser<int>.Parse(logger, parameters[WidthPxIdx], (param) => int.Parse(param), (val) => val > 0,
                $"{(WidthPxIdx + 1).ToOrdinal()} parameter must be a positive integer, but \"{parameters[WidthPxIdx]}\" is not."),
                Parser<int>.Parse(logger, parameters[HeightPxIdx], (param) => int.Parse(param), (val) => val > 0,
                $"{(HeightPxIdx + 1).ToOrdinal()} parameter must be a positive integer, but \"{parameters[HeightPxIdx]}\" is not."),
                Parser<double>.Parse(logger, parameters[WidthInchIdx], (param) => double.Parse(param.Replace(',', '.'), CultureInfo.InvariantCulture), (val) => val > 0,
                $"{(WidthInchIdx + 1).ToOrdinal()} parameter must be a positive number, but \"{parameters[WidthInchIdx]}\" is not."),
                Parser<double>.Parse(logger, parameters[HeightInchIdx], (param) => double.Parse(param.Replace(',', '.'), CultureInfo.InvariantCulture), (val) => val > 0,
                $"{(HeightInchIdx + 1).ToOrdinal()} parameter must be a positive number, but \"{parameters[HeightInchIdx]}\" is not."),
                InitElementSchemas(logger, fontProvider, imageProvider, elementSchemasParams, directory, TryGetColor(parameters[ColorIdx]), TryGetColor(parameters[ShadowColorIdx]), generateImages),
                TryGetColor(parameters[ColorIdx]),
                TryGetColor(parameters[ShadowColorIdx]),
                GetCommentIdxs(elementSchemasParams)
            )
        { }

        private static HashSet<int> GetCommentIdxs(List<List<string>> elementSchemasParams)
        {
            var i = 0;
            var commentIdxs = elementSchemasParams
                .Select(elementSchemaParams => new KeyValuePair<int, string>(i++, elementSchemaParams.First()))
                .Where(kv => string.IsNullOrEmpty(kv.Value))
                .Select(kv => kv.Key);

            return new HashSet<int>(commentIdxs);
        }

        private static List<ElementSchema> InitElementSchemas(ILogger logger, IFontProvider fontProvider, IImageProvider imageProvider, List<List<string>> elementSchemasParams, string directory, Color? defaultColor, Color? defaultShadowColor, bool generateImages)
        {
            var elementSchemas = new List<ElementSchema>(elementSchemasParams.Where(elementSchemaParams => !string.IsNullOrEmpty(elementSchemaParams.First())).Select(elementSchemaParams => new ElementSchema(logger, imageProvider, fontProvider, elementSchemaParams, directory, defaultColor ?? Color.Black, defaultShadowColor ?? Color.White, generateImages)));

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
                Background?.Dispose();
                foreach (var elementSchema in this)
                    elementSchema.Dispose();
            }

            disposed = true;
        }
    }
}

using CardCreator.Features.Drawing.Model;
using CardCreator.Features.Fonts;
using CardCreator.Features.Drawing;
using CardCreator.Features.Logging;
using CardCreator.Features.System;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace CardCreator.Features.Cards.Model
{
    public sealed class ElementSchema : IDisposable
    {
        public const int ParamsNumber = 15;

        private const int NameIdx = 0;
        private const int BackgroundIdx = 1;
        private const int XIdx = 2;
        private const int YIdx = 3;
        private const int WidthIdx = 4;
        private const int HeightIdx = 5;
        private const int ColorIdx = 6;
        private const int ShadowColorIdx = 7;
        private const int ShadowSizeIdx = 8;
        private const int FontIdx = 9;
        private const int MaxSizeIdx = 10;
        private const int HorizontalAlignmentIdx = 11;
        private const int VerticalAlignmentIdx = 12;
        private const int WrapIdx = 13;
        private const int JoinDirectionIdx = 14;

        public Image Background { get; }
        public string Name { get; }
        public Rectangle Area { get; }
        public Color Color { get; }
        public Color ShadowColor { get; }
        public int ShadowSize { get; }
        public FontFamily FontFamily { get; }
        public int MaxSize { get; }
        public int MinSize { get; }
        public StringFormatExtended StringFormat { get; }
        public bool WrapLines { get; }
        public JoinDirection JoinDirection { get; }

        private bool disposed = false;

        public ElementSchema(string name, Image background, Rectangle area, Color color, Color shadowColor, int shadowSize,
            FontFamily fontFamily, int maxSize, StringFormatExtended stringFormat, bool wrapLines, JoinDirection joinDirection)
        {
            Name = name;
            Background = background;
            Area = area;
            Color = color;
            ShadowColor = shadowColor;
            ShadowSize = shadowSize;
            FontFamily = fontFamily;
            MaxSize = maxSize;
            MinSize = Math.Min(6, MaxSize);
            StringFormat = stringFormat;
            WrapLines = wrapLines;
            JoinDirection = joinDirection;
        }

        public ElementSchema(string name, Image background, int x, int y, int width, int height, Color color, Color shadowColor, int shadowSize, FontFamily fontFamily,
            int maxSize, StringFormatExtended stringFormat, bool wrapLines, JoinDirection joinDirection)
            : this(name, background, new Rectangle(x, y, width, height), color, shadowColor, shadowSize, fontFamily, maxSize, stringFormat, wrapLines, joinDirection) { }

        public ElementSchema(ILogger logger, IImageProvider imageProvider, IFontProvider fontProvider,
            IList<string> parameters, string directory, Color defaultColor, Color defaultBorderColor, bool generateImages = true) :
            this(
                parameters[NameIdx],
                generateImages ? imageProvider.TryGet(Path.Combine(directory, parameters[BackgroundIdx])) ?? imageProvider.TryGetImageFromColor(parameters[BackgroundIdx], parameters[WidthIdx], parameters[HeightIdx]) : null,
                Parser<int>.Parse(logger, parameters[XIdx], (param) => string.IsNullOrEmpty(param) ? 0 : int.Parse(param), (val) => val >= 0,
                $"{(XIdx + 1).ToOrdinal()} parameter must be a nonnegative integer, but \"{parameters[XIdx]}\" is not."),
                Parser<int>.Parse(logger, parameters[YIdx], (param) => string.IsNullOrEmpty(param) ? 0 : int.Parse(param), (val) => val >= 0,
                $"{(YIdx + 1).ToOrdinal()} parameter must be a nonnegative integer, but \"{parameters[YIdx]}\" is not."),
                Parser<int>.Parse(logger, parameters[WidthIdx], (param) => string.IsNullOrEmpty(param) ? 0 : int.Parse(param), (val) => val >= 0,
                $"{(WidthIdx + 1).ToOrdinal()} parameter must be a nonnegative integer, but \"{parameters[WidthIdx]}\" is not."),
                Parser<int>.Parse(logger, parameters[HeightIdx], (param) => string.IsNullOrEmpty(param) ? 0 : int.Parse(param), (val) => val >= 0,
                $"{(HeightIdx + 1).ToOrdinal()} parameter must be a nonnegative integer, but \"{parameters[HeightIdx]}\" is not."),
                TryGetColor(parameters[ColorIdx], defaultColor),
                TryGetColor(parameters[ShadowColorIdx], defaultBorderColor),
                Parser<int>.Parse(logger, parameters[ShadowSizeIdx], (param) => int.TryParse(param, out var val) ? val : 0, (val) => val >= 0,
                $"{(ShadowSizeIdx + 1).ToOrdinal()} parameter must be a nonnegative integer, but \"{parameters[ShadowSizeIdx]}\" is not."),
                fontProvider.TryGet(parameters[FontIdx]),
                Parser<int>.Parse(logger, parameters[MaxSizeIdx], (param) => string.IsNullOrEmpty(param) ? 12 : int.Parse(param), (val) => val >= 0,
                $"{(MaxSizeIdx + 1).ToOrdinal()} parameter must be a nonnegative integer, but \"{parameters[MaxSizeIdx]}\" is not."),
                new StringFormatExtended(parameters[HorizontalAlignmentIdx], parameters[VerticalAlignmentIdx]),
                Parser<bool>.Parse(logger, parameters[WrapIdx], (param) => string.IsNullOrEmpty(param) ? true : bool.Parse(param), _ => true,
                $"{(WrapIdx + 1).ToOrdinal()} parameter must be a boolean, but \"{parameters[WrapIdx]}\" is not."),
                TryGetJoinDirection(parameters[JoinDirectionIdx])
            )
        { }

        private static Color TryGetColor(string color, Color @default)
        {
            if (string.IsNullOrEmpty(color)) return @default;
            try
            {
                return ColorTranslator.FromHtml(color);
            }
            catch
            {
                return Color.Black;
            }
        }

        private static JoinDirection TryGetJoinDirection(string direction)
        {
            if (string.IsNullOrEmpty(direction))
                return JoinDirection.None;

            switch (direction.Substring(0, 1).ToUpper())
            {
                case "H":
                    return JoinDirection.Horizontally;
                case "V":
                case "W":
                    return JoinDirection.Vertically;
                default:
                    return JoinDirection.None;
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
                Background?.Dispose();

            disposed = true;
        }
    }
}

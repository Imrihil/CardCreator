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
        public const int ParamsNumber = 14;

        private const int NameIdx = 0;
        private const int BackgroundIdx = 1;
        private const int XIdx = 2;
        private const int YIdx = 3;
        private const int WidthIdx = 4;
        private const int HeightIdx = 5;
        private const int ColorIdx = 6;
        private const int FontIdx = 7;
        private const int MaxSizeIdx = 8;
        private const int HorizontalAlignmentIdx = 9;
        private const int VerticalAlignmentIdx = 10;
        private const int WrapIdx = 11;
        private const int StretchIdx = 12;
        private const int JoinDirectionIdx = 13;

        public Image Background { get; }
        public string Name { get; }
        public Rectangle Area { get; }
        public Color Color { get; }
        public FontFamily Font { get; }
        public int MaxSize { get; }
        public int MinSize { get; }
        public StringFormat StringFormat { get; }
        public bool Wrap { get; }
        public bool StretchImage { get; }
        public JoinDirection JoinDirection { get; }

        public ElementSchema(string name, Image background, Rectangle area, Color color, FontFamily font, 
            int maxSize, StringFormat stringFormat, bool wrap, bool stretchImage, JoinDirection joinDirection)
        {
            Name = name;
            Background = background;
            Area = area;
            Color = color;
            Font = font;
            MaxSize = maxSize;
            MinSize = Math.Min(6, MaxSize);
            StringFormat = stringFormat;
            Wrap = wrap;
            StretchImage = stretchImage;
            JoinDirection = joinDirection;
        }

        public ElementSchema(string name, Image background, int x, int y, int width, int height, Color color, FontFamily font,
            int maxSize, StringFormat stringFormat, bool wrap, bool stretchImage, JoinDirection joinDirection)
            : this(name, background, new Rectangle(x, y, width, height), color, font, maxSize, stringFormat, wrap, stretchImage, joinDirection) { }

        public ElementSchema(ILogger logger, IImageProvider imageProvider, IFontProvider fontProvider,
            IList<string> parameters, string directory, Color? defaultColor = null) :
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
                GetStringFormat(parameters[HorizontalAlignmentIdx], parameters[VerticalAlignmentIdx]),
                Parser<bool>.Parse(logger, parameters[WrapIdx], (param) => string.IsNullOrEmpty(param) ? true : bool.Parse(param), _ => true,
                $"{(WrapIdx + 1).ToOrdinal()} parameter must be a boolean, but \"{parameters[WrapIdx]}\" is not."),
                Parser<bool>.Parse(logger, parameters[StretchIdx], (param) => string.IsNullOrEmpty(param) ? false : bool.Parse(param), _ => true,
                $"{(StretchIdx + 1).ToOrdinal()} parameter must be a boolean, but \"{parameters[StretchIdx]}\" is not."),
                TryGetJoinDirection(parameters[JoinDirectionIdx])
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

        private static StringFormat GetStringFormat(string horizontalAlignment, string verticalAlignment)
        {
            StringAlignment horizontal = GetAlignment(horizontalAlignment);
            StringAlignment vertical = GetAlignment(verticalAlignment);
            return new StringFormat
            {
                Alignment = horizontal,
                LineAlignment = vertical
            };
        }

        private static StringAlignment GetAlignment(string value)
        {
            if (string.IsNullOrEmpty(value))
                return StringAlignment.Center;

            switch (value.Substring(0, 1).ToUpper())
            {
                case "L": // Left
                case "T": // Top
                case "N": // Near
                    return StringAlignment.Near;
                case "C": // Center
                    return StringAlignment.Center;
                case "R": // Right
                case "B": // Bottom
                case "F": // Far
                    return StringAlignment.Far;
                default: throw new ArgumentException($"{value} is unknown alignment. Try: \"Left\", \"Top\" or \"Near\", \"Center\", \"Right\", \"Bottom\" or \"Far\".");
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
    }
}

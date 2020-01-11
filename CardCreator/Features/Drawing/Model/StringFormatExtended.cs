using System;
using System.Drawing;

namespace CardCreator.Features.Drawing.Model
{
    public class StringFormatExtended
    {
        public StringAlignmentExtended Alignment { get; set; }
        public StringAlignmentExtended LineAlignment { get; set; }
        public StringFormat StringFormat => new StringFormat
        {
            Alignment = GetStringAlignment(Alignment),
            LineAlignment = GetStringAlignment(LineAlignment)
        };
        public bool IsExtended => Alignment == StringAlignmentExtended.Justify || LineAlignment == StringAlignmentExtended.Justify;

        public StringFormatExtended(StringAlignmentExtended alignment, StringAlignmentExtended lineAlignment)
        {
            Alignment = alignment;
            LineAlignment = lineAlignment;
        }

        public StringFormatExtended(string alignment, string lineAlignment) : this(GetAlignment(alignment), GetAlignment(lineAlignment)) { }

        private static StringAlignmentExtended GetAlignment(string value)
        {
            if (string.IsNullOrEmpty(value))
                return StringAlignmentExtended.Center;

            switch (value.Substring(0, 1).ToUpper())
            {
                case "L": // Left
                case "T": // Top
                case "N": // Near
                    return StringAlignmentExtended.Near;
                case "C": // Center
                    return StringAlignmentExtended.Center;
                case "R": // Right
                case "B": // Bottom
                case "F": // Far
                    return StringAlignmentExtended.Far;
                case "J": // Justify
                    return StringAlignmentExtended.Justify;
                default: throw new ArgumentException($"{value} is unknown alignment. Try: \"Left\", \"Top\" or \"Near\"; \"Center\"; \"Right\", \"Bottom\" or \"Far\"; \"Justify\".");
            }
        }

        private StringAlignment GetStringAlignment(StringAlignmentExtended alignment) =>
            alignment switch
            {
                StringAlignmentExtended.Center => StringAlignment.Center,
                StringAlignmentExtended.Far => StringAlignment.Far,
                _ => StringAlignment.Near
            };

    }
}

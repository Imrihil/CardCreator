using CardCreator.Features.Drawing.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CardCreator.Features.Drawing
{
    public static class GraphicsJustifyExtensions
    {

        public static void DrawAdjustedStringWithShadow(this Graphics graphics, string s, FontFamily fontFamily, Color color, Color shadowColor, int shadowSize, RectangleF layoutRectangle, int maxFontSize, StringFormatExtended format = default, int minFontSize = 0, bool smallestOnFail = true, bool wordWrap = true)
        {
            if (!format.IsExtended || wordWrap == false)
            {
                graphics.DrawAdjustedStringWithShadow(s, fontFamily, color, shadowColor, shadowSize, layoutRectangle, maxFontSize, format.StringFormat, minFontSize, smallestOnFail, wordWrap);
                return;
            }
            var font = graphics.GetAdjustedFont(s, fontFamily, layoutRectangle, format.StringFormat, maxFontSize, minFontSize, smallestOnFail, wordWrap);
            var paragraphs = s.Split(new[] { "\n", "\r\n" }, StringSplitOptions.None)
                .Select(paragraph => graphics.GetLines(paragraph, font, layoutRectangle.Width));
            var allLines = paragraphs.Aggregate(0, (sum, paragraph) => sum += paragraph.Count);
            var i = 0;
            foreach (var paragraph in paragraphs)
            {
                var j = 0;
                foreach (var line in paragraph)
                {
                    var rectangle = GetVerticalLayoutRectangle(i, allLines, font.Height, layoutRectangle, format);
                    if (++j == paragraph.Count || format.Alignment != StringAlignmentExtended.Justify)
                        graphics.DrawStringWithShadow(line, font, color, shadowColor, shadowSize, rectangle, format.StringFormat);
                    else
                        graphics.DrawStretchStringWithShadow(line, font, color, shadowColor, shadowSize, rectangle);
                    ++i;
                }
            }
        }

        private static List<string> GetLines(this Graphics graphics, string s, Font font, float layoutWidth)
        {
            var lines = new List<string>();
            var remainingLine = s;
            while (!string.IsNullOrEmpty(remainingLine))
            {
                var nextLine = graphics.GetNextLine(remainingLine, font, layoutWidth);
                lines.Add(nextLine);
                remainingLine = remainingLine.Substring(nextLine.Length).TrimStart(' ');
            }

            return lines;
        }

        private static string GetNextLine(this Graphics graphics, string paragraph, Font font, float layoutWidth)
        { // abc def 
            paragraph += " ";
            var wordsIdx = paragraph.IndexesOf(' '); // 3 7

            var lIdx = 1; // 1 -> 7
            var rIdx = wordsIdx.Count - 1; // 2 - 1 = 1 -> 7
            var nextLine = paragraph.Substring(0, wordsIdx[0]); // abc
            while (lIdx <= rIdx)
            {
                var mIdx = (lIdx + rIdx) / 2; // 1
                var line = paragraph.Substring(0, wordsIdx[mIdx]); // 1 -> 7 -> abc def
                if (graphics.MeasureString(line, font).Width < layoutWidth)
                {
                    nextLine = line;
                    lIdx = mIdx + 1;
                }
                else
                {
                    rIdx = mIdx - 1;
                }
            }

            return nextLine;
        }

        private static List<int> IndexesOf(this string s, char c)
        {
            var idxs = new List<int>();
            for (var i = s.IndexOf(c); i > -1; i = s.IndexOf(c, i + 1))
                idxs.Add(i);
            return idxs;
        }

        private static RectangleF GetVerticalLayoutRectangle(int i, int n, int height, RectangleF layoutRectangle, StringFormatExtended format)
        {
            return format.LineAlignment switch
            {
                StringAlignmentExtended.Justify => new RectangleF(
                    layoutRectangle.X,
                    i == n - 1 ?
                        layoutRectangle.Y + layoutRectangle.Height - height :
                        layoutRectangle.Y + i * ((layoutRectangle.Height - height) / (n - 1)),
                    layoutRectangle.Width,
                    height),
                _ => new RectangleF(layoutRectangle.X, layoutRectangle.Y + i * height, layoutRectangle.Width, height)
            };
        }

        private static void DrawStretchStringWithShadow(this Graphics graphics, string s, Font font, Color color, Color shadowColor, int shadowSize, RectangleF layoutRectangle)
        {
            var words = s.Split(' ').Select(word => new KeyValuePair<string, SizeF>(word, graphics.MeasureString(word, font))).ToList();
            if (words.Count == 1)
            {
                graphics.DrawStringWithShadow(s, font, color, shadowColor, shadowSize, layoutRectangle);
                return;
            }
            var wordsWidth = words.Aggregate((float)0.0, (sum, word) => sum += word.Value.Width);
            var spacesWidth = layoutRectangle.Width - wordsWidth;
            var spaceWidth = spacesWidth / (words.Count - 1);
            var x = layoutRectangle.X;
            foreach (var word in words)
            {
                graphics.DrawStringWithShadow(word.Key, font, color, shadowColor, shadowSize, new RectangleF(x, layoutRectangle.Y, word.Value.Width, word.Value.Height));
                x += word.Value.Width + spaceWidth;
            }
        }
    }
}

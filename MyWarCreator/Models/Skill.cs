using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MyWarCreator.Extensions;
using MyWarCreator.Helpers;

namespace MyWarCreator.Models
{
    public class Skill : Card
    {
        private string Statistic { get; }
        private Rectangle StatisticArea { get; } = new Rectangle(25, 460, 120, 15);
        private string Attribute { get; }
        private Rectangle AttributeArea { get; } = new Rectangle(215, 460, 120, 15);
        private int Lvl { get; }
        private Rectangle LvlArea { get; } = new Rectangle(70, 460, 220, 15);
        private string Critical { get; }
        private Image CriticalImage { get; }
        private Rectangle CriticalArea { get; } = new Rectangle(17, 270, 50, 50);
        private Rectangle CriticalNameArea { get; } = new Rectangle(17, 295, 50, 20);
        protected override string FileName => $"{Statistic} - {Attribute} {Lvl} - {Name}";
        private bool IsOffensive { get; }
        private string FirstType { get; }
        private string FirstDescription { get; }
        private Rectangle FirstDescriptionArea { get; } = new Rectangle(30, 285, 300, 170);
        private string SecondType { get; }
        private string SecondDescription { get; }
        private Rectangle SecondDescriptionArea { get; } = new Rectangle(30, 415, 300, 40);

        public Skill(IList<string> row, string dirPath) : base(dirPath)
        {
            if (row.Count < 17)
                throw new ArgumentException("W wierszu znajduje się za mało kolumn by utworzyć kartę!");

            int.TryParse(row[0], out var value);
            Lvl = value;
            Statistic = row[1];
            Attribute = row[2];
            Name = row[3];
            for (var i = 4; i < 4 + 5; ++i)
            {
                if (!string.IsNullOrEmpty(row[i]))
                    LeftEffects.Add(row[i] + "+");
            }
            for (var i = 9; i < 9 + 5; ++i)
            {
                if (!string.IsNullOrEmpty(row[i]))
                    RightEffects.Add(row[i].Replace("+", "\n+"));
            }
            if (row[14].ToLower() == "tak")
            {
                IsOffensive = true;
            }
            if (IsOffensive)
            {
                const string leftEffectsImagePath = CardsDirPath + "/left-atk.png";
                if (File.Exists(leftEffectsImagePath))
                    LeftEffectsImage = Image.FromFile(leftEffectsImagePath);
            }
            FirstType = row[15];
            FirstDescription = row[16];
            Critical = row[17];
            SecondType = row[18];
            SecondDescription = row[19];
            if (!string.IsNullOrEmpty(SecondDescription))
                FirstDescriptionArea = new Rectangle(30, 285, 300, 125);

            MainImage = ImageHelper.LoadImage(dirPath, Name);
            CriticalImage = ImageHelper.LoadImage(CardsDirPath, Critical.Trim('.'));
        }

        protected override void DrawCard(Graphics graphics)
        {
            base.DrawCard(graphics);
            if (CriticalImage != null)
            {
                DrawingHelper.MapDrawing(graphics, CriticalImage, CriticalArea);
                //using (var font = new Font(FontTrebuchetMs, 12, FontStyle.Regular, GraphicsUnit.Pixel))
                //    graphics.DrawAdjustedStringWithExtendedBorder(Critical, font, Color.White, Color.Black, CriticalNameArea, FontsHelper.StringFormatCentered, 6);
            }
            else
            {
                using (var font = new Font(FontTrebuchetMs, 12, FontStyle.Regular, GraphicsUnit.Pixel))
                    graphics.DrawAdjustedStringWithExtendedBorder(Critical, font, Color.White, Color.Black, CriticalArea, FontsHelper.StringFormatCentered, 6);
            }
            using (var font = new Font(FontTrebuchetMs, 12, FontStyle.Regular, GraphicsUnit.Pixel))
                graphics.DrawAdjustedStringWithExtendedBorder(Statistic, font, Color.White, Color.Black, StatisticArea, FontsHelper.StringFormatLeft, 6);
            using (var font = new Font(FontTrebuchetMs, 12, FontStyle.Regular, GraphicsUnit.Pixel))
                graphics.DrawAdjustedStringWithExtendedBorder(Attribute, font, Color.White, Color.Black, AttributeArea, FontsHelper.StringFormatRight, 6);
            using (var font = new Font(FontTrebuchetMs, 12, FontStyle.Regular, GraphicsUnit.Pixel))
                graphics.DrawAdjustedStringWithExtendedBorder(DescriptionHelper.ToRoman(Lvl), font, Color.White, Color.Black, LvlArea, FontsHelper.StringFormatCentered, 6);
            using (var font = new Font(FontTrebuchetMs, 12, FontStyle.Regular, GraphicsUnit.Pixel))
                graphics.DrawAdjustedStringWithExtendedBorder(
                    Regex.IsMatch(FirstType, "(?i)(pasywna)")
                        ? $"{FirstDescription}"
                        : $"{FirstType} {FirstDescription}",
                    font, Color.White, Color.Black, FirstDescriptionArea, FontsHelper.StringFormatCentered, 6);
            using (var font = new Font(FontTrebuchetMs,
                12, FontStyle.Regular, GraphicsUnit.Pixel))
                graphics.DrawAdjustedStringWithExtendedBorder(
                    Regex.IsMatch(SecondType, "(?i)(pasywna)")
                    ? $"{SecondDescription}"
                    : $"{SecondType} {SecondDescription}",
                    font, Color.White, Color.Black, SecondDescriptionArea, FontsHelper.StringFormatCentered, 6);
        }
    }
}

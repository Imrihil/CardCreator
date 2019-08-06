using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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
        private Rectangle CriticalArea { get; } = new Rectangle(70, 25, 50, 50);
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

            MainImage = LoadImage(dirPath, Name);
            CriticalImage = LoadImage(CardsDirPath, Critical.Trim('.'));
        }

        protected override void DrawCard(Graphics graphics)
        {
            base.DrawCard(graphics);
            if (CriticalImage != null)
            {
                DrawingHelper.MapDrawing(graphics, CriticalImage, CriticalArea);
            }
            else
            {
                using (var font = new Font(FontsHelper.Pfc.Families.FirstOrDefault(x => x.Name.Contains("Trebuchet MS")) ?? FontFamily.GenericSansSerif,
                    12, FontStyle.Regular, GraphicsUnit.Pixel))
                    graphics.DrawAdjustedString(Critical, font, Brushes.White, CriticalArea, FontsHelper.StringFormatCentered, 6);
            }
            using (var font = new Font(FontsHelper.Pfc.Families.FirstOrDefault(x => x.Name.Contains("Trebuchet MS")) ?? FontFamily.GenericSansSerif,
                12, FontStyle.Regular, GraphicsUnit.Pixel))
                graphics.DrawAdjustedString(Statistic, font, Brushes.White, StatisticArea, FontsHelper.StringFormatLeft, 6);
            using (var font = new Font(FontsHelper.Pfc.Families.FirstOrDefault(x => x.Name.Contains("Trebuchet MS")) ?? FontFamily.GenericSansSerif,
                12, FontStyle.Regular, GraphicsUnit.Pixel))
                graphics.DrawAdjustedString(Attribute, font, Brushes.White, AttributeArea, FontsHelper.StringFormatRight, 6);
            using (var font = new Font(FontsHelper.Pfc.Families.FirstOrDefault(x => x.Name.Contains("Trebuchet MS")) ?? FontFamily.GenericSansSerif,
                12, FontStyle.Regular, GraphicsUnit.Pixel))
                graphics.DrawAdjustedString(DescriptionHelper.ToRoman(Lvl), font, Brushes.White, LvlArea, FontsHelper.StringFormatCentered, 6);
            using (var font = new Font(FontsHelper.Pfc.Families.FirstOrDefault(x => x.Name.Contains("Trebuchet MS")) ?? FontFamily.GenericSansSerif,
                12, FontStyle.Regular, GraphicsUnit.Pixel))
                graphics.DrawAdjustedString($"{FirstType} {FirstDescription}", font, Brushes.White, FirstDescriptionArea, FontsHelper.StringFormatCentered, 6);
            using (var font = new Font(FontsHelper.Pfc.Families.FirstOrDefault(x => x.Name.Contains("Trebuchet MS")) ?? FontFamily.GenericSansSerif,
                12, FontStyle.Regular, GraphicsUnit.Pixel))
                graphics.DrawAdjustedString($"{SecondType} {SecondDescription}", font, Brushes.White, SecondDescriptionArea, FontsHelper.StringFormatCentered, 6);
        }
    }
}

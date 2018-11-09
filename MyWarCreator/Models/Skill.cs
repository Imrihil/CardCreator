using MyWarCreator.Extensions;
using MyWarCreator.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWarCreator.Models
{
    class Skill : Card
    {
        public string Statistic { get; set; }
        public Rectangle StatisticArea { get; set; } = new Rectangle(25, 460, 120, 15);
        public string Attribute { get; set; }
        public Rectangle AttributeArea { get; set; } = new Rectangle(215, 460, 120, 15);
        public int Lvl { get; set; }
        public Rectangle LvlArea { get; set; } = new Rectangle(70, 460, 220, 15);
        public string Critical { get; set; }
        public Image CriticalImage { get; set; }
        public Rectangle CriticalArea { get; set; } = new Rectangle(70, 25, 50, 50);
        public override string FileName { get { return $"{Statistic} - {Attribute} {Lvl} - {Name}"; } }
        public bool IsOffensive { get; set; }
        public string FirstType { get; set; }
        public Image FirstTypeImage { get; set; }
        public Rectangle FirstTypeImageArea { get; set; } = new Rectangle(30, 350, 40, 40);
        public string FirstDescription { get; set; }
        public Rectangle FirstDescriptionArea { get; set; } = new Rectangle(70, 285, 260, 170);
        public string SecondType { get; set; }
        public Image SecondTypeImage { get; set; }
        public Rectangle SecondTypeImageArea { get; set; } = new Rectangle(30, 415, 40, 40);
        public string SecondDescription { get; set; }
        public Rectangle SecondDescriptionArea { get; set; } = new Rectangle(70, 415, 260, 40);

        public Skill(IList<string> row, string dirPath) : base(dirPath)
        {
            if (row.Count < 17)
                throw new ArgumentException("W wierszu znajduje się za mało kolumn by utworzyć kartę!");

            int value;
            int.TryParse(row[0], out value);
            Lvl = value;
            Statistic = row[1];
            Attribute = row[2];
            Name = row[3];
            for (int i = 4; i < 4 + 5; ++i)
            {
                if (!string.IsNullOrEmpty(row[i]))
                    LeftEffects.Add(row[i] + "+");
            }
            for (int i = 9; i < 9 + 5; ++i)
            {
                if (!string.IsNullOrEmpty(row[i]))
                    RightEffects.Add(row[i].Replace("+", "\n+"));
            }
            if (row[14].ToLower() == "tak")
            {
                IsOffensive = true;
                string leftEffectsImagePath = cardsDirPath + "/left-atk.png";
                if (File.Exists(leftEffectsImagePath))
                    LeftEffectsImage = Image.FromFile(leftEffectsImagePath);
            }
            FirstType = row[15];
            FirstDescription = row[16];
            Critical = row[17];
            SecondType = row[18];
            SecondDescription = row[19];

            if (!string.IsNullOrEmpty(SecondDescription))
            {
                FirstTypeImageArea = new Rectangle(30, 327, 40, 40);
                FirstDescriptionArea = new Rectangle(70, 285, 260, 125);
            }

            MainImage = LoadImage(dirPath, Name);
            CriticalImage = LoadImage(cardsDirPath, Critical.Trim('.'));
            FirstTypeImage = LoadImage(cardsDirPath, FirstType.Trim('.'));
            SecondTypeImage = LoadImage(cardsDirPath, SecondType.Trim('.'));
        }

        public override void DrawCard(Graphics graphics)
        {
            base.DrawCard(graphics);
            if (CriticalImage != null)
            {
                DrawingHelper.MapDrawing(graphics, CriticalImage, CriticalArea);
            }
            else
            {
                using (Font font = new Font(FontsHelper.pfc.Families.FirstOrDefault(x => x.Name.Contains("Trebuchet MS")), 12, FontStyle.Regular, GraphicsUnit.Pixel))
                    graphics.DrawAdjustedString(Critical, font, Brushes.White, CriticalArea, FontsHelper.StringFormatCentered, 6);
            }
            using (Font font = new Font(FontsHelper.pfc.Families.FirstOrDefault(x => x.Name.Contains("Trebuchet MS")), 12, FontStyle.Regular, GraphicsUnit.Pixel))
                graphics.DrawAdjustedString(Statistic, font, Brushes.White, StatisticArea, FontsHelper.StringFormatLeft, 6);
            using (Font font = new Font(FontsHelper.pfc.Families.FirstOrDefault(x => x.Name.Contains("Trebuchet MS")), 12, FontStyle.Regular, GraphicsUnit.Pixel))
                graphics.DrawAdjustedString(Attribute, font, Brushes.White, AttributeArea, FontsHelper.StringFormatRight, 6);
            using (Font font = new Font(FontsHelper.pfc.Families.FirstOrDefault(x => x.Name.Contains("Trebuchet MS")), 12, FontStyle.Regular, GraphicsUnit.Pixel))
                graphics.DrawAdjustedString(DescriptionHelper.ToRoman(Lvl), font, Brushes.White, LvlArea, FontsHelper.StringFormatCentered, 6);
            if (FirstTypeImage != null)
            {
                DrawingHelper.MapDrawing(graphics, FirstTypeImage, FirstTypeImageArea);
            }
            else
            {
                using (Font font = new Font(FontsHelper.pfc.Families.FirstOrDefault(x => x.Name.Contains("Trebuchet MS")), 12, FontStyle.Regular, GraphicsUnit.Pixel))
                    graphics.DrawAdjustedString(FirstType, font, Brushes.White, FirstTypeImageArea, FontsHelper.StringFormatCentered, 6);
            }
            using (Font font = new Font(FontsHelper.pfc.Families.FirstOrDefault(x => x.Name.Contains("Trebuchet MS")), 12, FontStyle.Regular, GraphicsUnit.Pixel))
                graphics.DrawAdjustedString(FirstDescription, font, Brushes.White, FirstDescriptionArea, FontsHelper.StringFormatCentered, 6);
            if (SecondTypeImage != null)
            {
                DrawingHelper.MapDrawing(graphics, SecondTypeImage, SecondTypeImageArea);
            }
            else
            {
                using (Font font = new Font(FontsHelper.pfc.Families.FirstOrDefault(x => x.Name.Contains("Trebuchet MS")), 12, FontStyle.Regular, GraphicsUnit.Pixel))
                    graphics.DrawAdjustedString(SecondType, font, Brushes.White, SecondTypeImageArea, FontsHelper.StringFormatCentered, 6);
            }
            using (Font font = new Font(FontsHelper.pfc.Families.FirstOrDefault(x => x.Name.Contains("Trebuchet MS")), 12, FontStyle.Regular, GraphicsUnit.Pixel))
                graphics.DrawAdjustedString(SecondDescription, font, Brushes.White, SecondDescriptionArea, FontsHelper.StringFormatCentered, 6);
        }
    }
}

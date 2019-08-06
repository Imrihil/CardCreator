using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MyWarCreator.Extensions;
using MyWarCreator.Helpers;

namespace MyWarCreator.Models
{
    public sealed class Equipment : AttackAbilityCard
    {
        private int Weight { get; }
        private Image WeightImage { get; }
        private Rectangle WeightImageArea { get; } = new Rectangle(300, 440, 40, 40);
        private bool IsArmour => Type == "Zbroja" || Type == "Hełm" || Type == "Buty" || Type == "Rękawice";

        protected override string DescriptionFull
        {
            get
            {
                if (IsArmour)
                    return base.DescriptionFull;

                var sb = new StringBuilder();
                if (!string.IsNullOrEmpty(Description))
                    sb.AppendFormat("{0}\n\n", Description);
                if (Defence > 0)
                    sb.AppendFormat("Obrona: {0}\n\n", Defence.ToString());
                sb.Append(AttackDescription());
                return sb.ToString();
            }
        }

        public Equipment(IList<string> row, string dirPath) : base(dirPath)
        {
            Type = row[0];
            Name = row[1];
            if (!string.IsNullOrEmpty(row[2]) ||
                !string.IsNullOrEmpty(row[3]) ||
                !string.IsNullOrEmpty(row[4]) ||
                !string.IsNullOrEmpty(row[5]) ||
                !string.IsNullOrEmpty(row[6]))
            {
                if (!string.IsNullOrEmpty(row[2])) LeftEffects.Add(row[2] + "S"); else LeftEffects.Add("");
                if (!string.IsNullOrEmpty(row[3])) LeftEffects.Add(row[3] + "Z"); else LeftEffects.Add("");
                if (!string.IsNullOrEmpty(row[4])) LeftEffects.Add(row[4] + "W"); else LeftEffects.Add("");
                if (!string.IsNullOrEmpty(row[5])) LeftEffects.Add(row[5] + "M"); else LeftEffects.Add("");
                if (!string.IsNullOrEmpty(row[6])) LeftEffects.Add(row[6] + "U"); else LeftEffects.Add("");
            }
            if (!string.IsNullOrEmpty(row[7]) ||
                !string.IsNullOrEmpty(row[8]) ||
                !string.IsNullOrEmpty(row[9]) ||
                !string.IsNullOrEmpty(row[10]) ||
                !string.IsNullOrEmpty(row[11]))
            {
                if (!string.IsNullOrEmpty(row[7])) RightEffects.Add((row[7] == "1" ? (row[29] == "TAK" ? "1" : "") : row[7]) + (row[29] == "TAK" ? "" : "k4")); else RightEffects.Add("");
                if (!string.IsNullOrEmpty(row[8])) RightEffects.Add((row[8] == "1" ? (row[29] == "TAK" ? "1" : "") : row[8]) + (row[29] == "TAK" ? "" : "k6")); else RightEffects.Add("");
                if (!string.IsNullOrEmpty(row[9])) RightEffects.Add((row[9] == "1" ? (row[29] == "TAK" ? "1" : "") : row[9]) + (row[29] == "TAK" ? "" : "k8")); else RightEffects.Add("");
                if (!string.IsNullOrEmpty(row[10])) RightEffects.Add((row[10] == "1" ? (row[29] == "TAK" ? "1" : "") : row[10]) + (row[29] == "TAK" ? "" : "k10")); else RightEffects.Add("");
                if (!string.IsNullOrEmpty(row[11])) RightEffects.Add((row[11] == "1" ? (row[29] == "TAK" ? "1" : "") : row[11]) + (row[29] == "TAK" ? "" : "k12")); else RightEffects.Add("");
            }
            ProcessRow(row.Skip(12).ToList());
            int.TryParse(row[30], out var value);
            Defence = value;
            if (IsArmour)
                RightEffects.Add(Defence.ToString());
            int.TryParse(row[31], out value);
            Weight = value;
            Description = row[32];
            int.TryParse(row[33], out value);
            Price = value;

            CalculateTypeArea();

            MainImage = LoadImage(dirPath, Name);
            LeftEffectsImage = LoadImage(CardsDirPath, "left-stats");
            WeightImage = LoadImage(CardsDirPath, "weight");
            if (IsArmour)
                RightEffectsImage = LoadImage(CardsDirPath, "right-armour");
        }

        protected override void CalculateTypeArea()
        {
            base.CalculateTypeArea();
            if (Weight <= 0) return;
            TypeArea = Weight <= 5 - (Price > PriceLimit ? 2 : Price)
                ? new Rectangle(TypeArea.X, TypeArea.Y, TypeArea.Width - WeightImageArea.Width * Weight, TypeArea.Height)
                : new Rectangle(TypeArea.X, TypeArea.Y, TypeArea.Width - WeightImageArea.Width * 2, TypeArea.Height);
        }

        protected override void DrawCard(Graphics graphics)
        {
            base.DrawCard(graphics);

            if (WeightImage != null)
            {
                if (Weight <= 0) return;

                if (Weight <= 5 - (Price > PriceLimit ? 2 : Price))
                {
                    for (var i = 0; i < Weight; ++i)
                    {
                        var weightImageAreaI = new Rectangle(WeightImageArea.X - i * WeightImageArea.Width, WeightImageArea.Y, WeightImageArea.Width, WeightImageArea.Height);
                        DrawingHelper.MapDrawing(graphics, WeightImage, weightImageAreaI);
                    }
                }
                else
                {
                    var weightImageAreaI = new Rectangle(WeightImageArea.X - WeightImageArea.Width + 5, WeightImageArea.Y, WeightImageArea.Width - 5, WeightImageArea.Height);
                    using (var font = new Font(FontsHelper.Pfc.Families.FirstOrDefault(x => x.Name.Contains("Trebuchet MS")) ?? FontFamily.GenericSansSerif,
                        12, FontStyle.Bold, GraphicsUnit.Pixel))
                        graphics.DrawAdjustedString(Weight.ToString(), font, Brushes.White, weightImageAreaI, FontsHelper.StringFormatCentered, 6, 12, true, false);
                    DrawingHelper.MapDrawing(graphics, WeightImage, WeightImageArea);
                }
            }
            else
            {
                using (var font = new Font(FontsHelper.Pfc.Families.FirstOrDefault(x => x.Name.Contains("Trebuchet MS")) ?? FontFamily.GenericSansSerif,
                    12, FontStyle.Bold, GraphicsUnit.Pixel))
                    graphics.DrawAdjustedString(Weight.ToString(), font, Brushes.White, WeightImageArea, FontsHelper.StringFormatCentered, 6, 12, true, false);
            }
        }
    }
}

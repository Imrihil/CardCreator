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
                if (Defense > 0)
                    sb.AppendFormat("Obrona: {0}\n\n", Defense.ToString());
                return sb.ToString();
            }
        }

        public Equipment(IList<string> row, string dirPath) : base(dirPath)
        {
            Type = row[0];
            Name = row[1];
            if (!string.IsNullOrEmpty(row[2])) RightEffects.Add(row[2] + "B");
            if (!string.IsNullOrEmpty(row[3])) RightEffects.Add(row[3] + "Z");
            if (!string.IsNullOrEmpty(row[4])) RightEffects.Add(row[4] + "W");
            if (!string.IsNullOrEmpty(row[5])) RightEffects.Add(row[5] + "M");
            if (!string.IsNullOrEmpty(row[6])) RightEffects.Add(row[6] + "U");
            if (!string.IsNullOrEmpty(row[7])) RightEffects.Add((row[7] == "1" ? "" : row[7]) + "k4");
            if (!string.IsNullOrEmpty(row[8])) RightEffects.Add((row[8] == "1" ? "" : row[8]) + "k6");
            if (!string.IsNullOrEmpty(row[9])) RightEffects.Add((row[9] == "1" ? "" : row[9]) + "k8");
            if (!string.IsNullOrEmpty(row[10])) RightEffects.Add((row[10] == "1" ? "" : row[10]) + "k10");
            if (!string.IsNullOrEmpty(row[11])) RightEffects.Add((row[11] == "1" ? "" : row[11]) + "k12");

            ProcessRow(row.Skip(12).ToList());
            int.TryParse(row[30], out var value);
            Defense = value;
            if (IsArmour)
                RightEffects.Add(Defense.ToString());
            int.TryParse(row[31], out value);
            Weight = value;
            Description = row[32];
            int.TryParse(row[33], out value);
            Price = value;

            CalculateTypeArea();

            MainImage = ImageHelper.LoadImage(dirPath, Name);
            LeftEffectsImage = ImageHelper.LoadImage(CardsDirPath, "left-stats");
            WeightImage = ImageHelper.LoadImage(CardsDirPath, "weight");
            if (IsArmour)
            {
                RightEffectsImage = ImageHelper.LoadImage(CardsDirPath, "right-armour");
            }
            else
            {
                RightEffectsImage = ImageHelper.LoadImage(CardsDirPath, "circle");
                RightEffectsImageArea = new Rectangle(280, 0, 80, 80);
            }
        }

        protected override void CalculateTypeArea()
        {
            base.CalculateTypeArea();
            if (Weight <= 0) return;
            TypeArea = Weight <= 5 - (Price > PriceLimit ? 2 : Price)
                ? new Rectangle(TypeArea.X, TypeArea.Y, TypeArea.Width - WeightImageArea.Width * Weight, TypeArea.Height)
                : new Rectangle(TypeArea.X, TypeArea.Y, TypeArea.Width - WeightImageArea.Width * 2, TypeArea.Height);
        }

        protected override void DrawCard(Graphics graphics, bool blackAndWhite)
        {
            base.DrawCard(graphics, blackAndWhite);

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
                    using (var font = new Font(FontTrebuchetMs, 12, FontStyle.Bold, GraphicsUnit.Pixel))
                        graphics.DrawAdjustedStringWithExtendedBorder(Weight.ToString(), font, GetColor(blackAndWhite), GetColor(!blackAndWhite), weightImageAreaI, FontsHelper.StringFormatCentered, 6, 12, true, false);
                    DrawingHelper.MapDrawing(graphics, WeightImage, WeightImageArea);
                }
            }
            else
            {
                using (var font = new Font(FontTrebuchetMs, 12, FontStyle.Bold, GraphicsUnit.Pixel))
                    graphics.DrawAdjustedStringWithExtendedBorder(Weight.ToString(), font, GetColor(blackAndWhite), GetColor(!blackAndWhite), WeightImageArea, FontsHelper.StringFormatCentered, 6, 12, true, false);
            }
        }

        protected override void DrawRightEffectsBackground(Graphics graphics)
        {
            for (var i = 0; i < RightEffects.Count; ++i)
            {
                var effectArea = new Rectangle(RightEffectsImageArea.X + RightEffectsAreaShift.X * i, RightEffectsImageArea.Y + RightEffectsAreaShift.Y * i, RightEffectsImageArea.Width, RightEffectsImageArea.Height);
                DrawingHelper.MapDrawing(graphics, RightEffectsImage, effectArea);
            }
        }
    }
}

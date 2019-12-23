using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MyWarCreator.Extensions;
using MyWarCreator.Helpers;

namespace MyWarCreator.Models
{
    public class AttackAbilityCard : Card
    {
        protected int Defense { get; set; }

        private List<AttackAbilityElement> Elements { get; }

        private int ElementsWithValues => Elements.Count(e => e.Value > 0);

        protected AttackAbilityCard(string dirPath) : base(dirPath)
        {
            Elements = new List<AttackAbilityElement>
            {
                new AttackAbilityElement($"{CardsDirPath}/atak", "Zmęczenie"),
                new AttackAbilityElement($"{CardsDirPath}/atak", "Swój"),
                new AttackAbilityElement($"{CardsDirPath}/atak", "Trafienie"),
                new AttackAbilityElement($"{CardsDirPath}/atak", "Spowolnienie"),
                new AttackAbilityElement($"{CardsDirPath}/atak", "Osłabienie"),
                new AttackAbilityElement($"{CardsDirPath}/atak", "Zatrucie"),
                new AttackAbilityElement($"{CardsDirPath}/atak", "Krwawienie"),
                new AttackAbilityElement($"{CardsDirPath}/atak", "Atak obszarowy"),
                new AttackAbilityElement($"{CardsDirPath}/atak", "Oślepienie"),
                new AttackAbilityElement($"{CardsDirPath}/atak", "Przebicie"),
                new AttackAbilityElement($"{CardsDirPath}/atak", "Podpalenie"),
                new AttackAbilityElement($"{CardsDirPath}/atak", "Wyciszenie"),
                new AttackAbilityElement($"{CardsDirPath}/atak", "Ogłuszenie"),
                new AttackAbilityElement($"{CardsDirPath}/atak", "Powalenie"),
                new AttackAbilityElement($"{CardsDirPath}/atak", "Hipnoza"),
                new AttackAbilityElement($"{CardsDirPath}/atak", "Odepchnięcie"),
                new AttackAbilityElement($"{CardsDirPath}/atak", "Strach"),
                new AttackAbilityElement($"{CardsDirPath}/atak", "Krytyk")
            };
        }

        protected string AttackDescription()
        {
            var sb = new StringBuilder();
            var actual = 1;
            foreach (var element in Elements)
            {
                if (element.Value > 0)
                    sb.AppendLine(ChancesLine(element.Name, element.Value, ref actual));
            }
            return sb.ToString();
        }

        protected void ProcessRow(IList<string> row)
        {
            for (var i = 0; i < Elements.Count; ++i)
            {
                int.TryParse(row[i], out var value);
                Elements[i].Value = value;
            }
        }

        protected override void DrawDescription(Graphics graphics, bool blackAndWhite)
        {
            if (ElementsWithValues > 0)
            {
                var elementArea = string.IsNullOrEmpty(Description)
                    ? new Rectangle(DescriptionArea.X, DescriptionArea.Y, DescriptionArea.Width,
                        (DescriptionArea.Height - 10) / ElementsWithValues)
                    : new Rectangle(DescriptionArea.X, DescriptionArea.Y,
                        2 * Math.Min(DescriptionArea.Height / ElementsWithValues, 100),
                        Math.Min((DescriptionArea.Height - 10) / ElementsWithValues, 100));
                DescriptionArea = new Rectangle(DescriptionArea.X + elementArea.Width + 5, DescriptionArea.Y,
                    DescriptionArea.Width - elementArea.Width - 5, DescriptionArea.Height);

                var actual = 1;
                foreach (var element in Elements.Where(element => element.Value > 0))
                {
                    DrawChancesLine(graphics, element, elementArea, ref actual, blackAndWhite);
                    elementArea = new Rectangle(elementArea.X, elementArea.Y + elementArea.Height,
                        elementArea.Width, elementArea.Height);
                }
            }

            if (!string.IsNullOrWhiteSpace(Description))
            {
                base.DrawDescription(graphics, blackAndWhite);
            }

        }

        private void DrawChancesLine(Graphics graphics, AttackAbilityElement element, Rectangle elementArea, ref int actual, bool blackAndWhite)
        {
            if (element.Value <= 0) return;

            string hits;
            var min = actual;
            actual += element.Value;
            if (actual > 12)
                hits = $"{min}+";
            else if (min == 1)
                hits = $"{element.Value}-";
            else
                hits = element.Value == 1 ? $"{min}" : $"{min}-{actual - 1}";

            var chancesAreaLeft = new Rectangle(elementArea.X, elementArea.Y + 1, elementArea.Width / 2 - 1, elementArea.Height - 2);
            var chancesAreaRight = new Rectangle(elementArea.X + elementArea.Width / 2 + 1, elementArea.Y + 1, elementArea.Width / 2 - 1, elementArea.Height - 2);

            using (var font = new Font(FontTrebuchetMs, 24, FontStyle.Regular, GraphicsUnit.Pixel))
            {
                graphics.DrawAdjustedStringWithExtendedBorder(hits, font, GetColor(blackAndWhite), GetColor(!blackAndWhite), chancesAreaLeft, FontsHelper.StringFormatRight, wordWrap: false);
                if (element.Image == null)
                    graphics.DrawAdjustedStringWithExtendedBorder($": {element.Name}", font, GetColor(blackAndWhite), GetColor(!blackAndWhite), chancesAreaRight, FontsHelper.StringFormatLeft);
            }
            if (element.Image != null)
                DrawingHelper.MapDrawing(graphics, element.Image, chancesAreaRight, center: false);
        }

        private static string ChancesLine(string name, int hitChance, ref int actual)
        {
            var min = actual;
            actual += hitChance;
            if (actual > 12)
                return $"{min}+: {name}";
            if (min == 1)
                return $"{hitChance}-: {name}";
            return hitChance == 1
                ? $"{min}: {name}"
                : $"{min}-{actual - 1}: {name}";
        }
    }
}

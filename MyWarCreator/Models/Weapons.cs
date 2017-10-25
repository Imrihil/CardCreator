using MyWarCreator.Extensions;
using MyWarCreator.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace MyWarCreator.Models
{
    class Weapon : Equipment
    {
        public int Fatigue { get; set; }
        public int Miss { get; set; }
        public int Hit { get; set; }
        public int Knockdown { get; set; }
        public int Stun { get; set; }
        public int Cleave { get; set; }
        public int Crit { get; set; }
        public override string DescriptionFull
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (!string.IsNullOrEmpty(Description))
                    sb.AppendLine(Description);
                int actual = 1;
                if (Fatigue > 0)
                    sb.AppendLine(ChancesLine("Zmęczenie", Fatigue, ref actual));
                if (Miss > 0)
                    sb.AppendLine(ChancesLine("Draśnięcie", Miss, ref actual));
                if (Hit > 0)
                    sb.AppendLine(ChancesLine("Zwykły cios", Hit, ref actual));
                if (Knockdown > 0)
                    sb.AppendLine(ChancesLine("Powalenie", Knockdown, ref actual));
                if (Stun > 0)
                    sb.AppendLine(ChancesLine("Ogłuszenie", Stun, ref actual));
                if (Cleave > 0)
                    sb.AppendLine(ChancesLine("Rozpłatanie", Cleave, ref actual));
                if (Crit > 0)
                    sb.AppendLine(ChancesLine("Krytyk", Crit, ref actual));
                return sb.ToString();
            }
        }
        private string ChancesLine(string name, int hitChance, ref int actual)
        {
            int min = actual;
            actual += hitChance;
            if (actual > 12)
                return string.Format("{0}+: {1}", min, name);
            else if (min == 1)
                return string.Format("{0}-: {1}", hitChance, name);
            else if (hitChance == 1)
                return string.Format("{0}: {1}", min, name);
            else
                return string.Format("{0}-{1}: {2}", min, actual - 1, name);
        }
        public Weapon(IList<string> row, string dirPath) : base(row, dirPath)
        {
            if (row[3] != "NULL") RunePlaces = "A\nB\nC\nD\nE";
            if (row.Count < 12)
                throw new ArgumentException("W wierszu znajduje się za mało kolumn by utworzyć kartę broni!");
            int value;
            try
            {
                int.TryParse(row[5], out value);
                Miss = value;
                int.TryParse(row[6], out value);
                Fatigue = value;
                int.TryParse(row[7], out value);
                Hit = value;
                int.TryParse(row[8], out value);
                Crit = value;
                int.TryParse(row[9], out value);
                Knockdown = value;
                int.TryParse(row[10], out value);
                Stun = value;
                int.TryParse(row[11], out value);
                Cleave = value;
            }
            catch (Exception)
            {
                throw new ArgumentException("Kolumny 5-11 powinny zawierać liczby!");
            }
            if ((Dmg.ToLower().Contains("d20") || Dmg.ToLower().Contains("k20")) && File.Exists(dicesDirPath + "/d20.png"))
                DiceImage = Image.FromFile(dicesDirPath + "/d20.png");
            else if ((Dmg.ToLower().Contains("d12") || Dmg.ToLower().Contains("k12")) && File.Exists(dicesDirPath + "/d12.png"))
                DiceImage = Image.FromFile(dicesDirPath + "/d12.png");
            else if ((Dmg.ToLower().Contains("d10") || Dmg.ToLower().Contains("k10")) && File.Exists(dicesDirPath + "/d10.png"))
                DiceImage = Image.FromFile(dicesDirPath + "/d10.png");
            else if ((Dmg.ToLower().Contains("d8") || Dmg.ToLower().Contains("k8")) && File.Exists(dicesDirPath + "/d8.png"))
                DiceImage = Image.FromFile(dicesDirPath + "/d8.png");
            else if ((Dmg.ToLower().Contains("d6") || Dmg.ToLower().Contains("k6")) && File.Exists(dicesDirPath + "/d6.png"))
                DiceImage = Image.FromFile(dicesDirPath + "/d6.png");
            else if ((Dmg.ToLower().Contains("d4") || Dmg.ToLower().Contains("k4")) && File.Exists(dicesDirPath + "/d4.png"))
                DiceImage = Image.FromFile(dicesDirPath + "/d4.png");
            else if (File.Exists(dicesDirPath + "/default.png"))
                DiceImage = Image.FromFile(dicesDirPath + "/default.png");
            if (File.Exists(dicesDirPath + "/upgrade.png"))
                UpgradesImage = Image.FromFile(dicesDirPath + "/upgrade.png");
        }
        public override void DrawCard(Graphics graphics)
        {
            base.DrawCard(graphics);
            if (Dmg != null)
            {
                using (Font font = new Font(FontsHelper.pfc.Families.FirstOrDefault(x => x.Name.Contains("Trebuchet MS")), 18, FontStyle.Bold, GraphicsUnit.Pixel))
                    graphics.DrawAdjustedString(Dmg, font, Brushes.Black, DiceTextArea, FontsHelper.StringFormatCentered, 6);
            }
        }
    }
}

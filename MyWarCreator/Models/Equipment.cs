using MyWarCreator.Helpers;
using MyWarCreator.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace MyWarCreator.Models
{
    class Equipment : Card
    {
        public int Fatigue { get; set; }
        public int FriendyFire { get; set; }
        public int Hit { get; set; }
        public int Downturn { get; set; }
        public int Knockdown { get; set; }
        public int Poison { get; set; }
        public int Weakness { get; set; }
        public int Stun { get; set; }
        public int Bleeding { get; set; }
        public int AreaAttack { get; set; }
        public int Cleave { get; set; }
        public int Fire { get; set; }
        public int Push { get; set; }
        public int Terror { get; set; }
        public int Confusion { get; set; }
        public int Hypnosis { get; set; }
        public int Crit { get; set; }
        public int Armour { get; set; }
        public override string DescriptionFull
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (!string.IsNullOrEmpty(Description))
                    sb.AppendFormat("{0}\n\n", Description);
                if (Armour > 0)
                    sb.AppendFormat("Obrona: {0}\n", Armour.ToString());
                int actual = 1;
                if (Fatigue > 0)
                    sb.AppendLine(ChancesLine("Zmęczenie", Fatigue, ref actual));
                if (FriendyFire > 0)
                    sb.AppendLine(ChancesLine("Swój", FriendyFire, ref actual));
                if (Hit > 0)
                    sb.AppendLine(ChancesLine("Trafienie", Hit, ref actual));
                if (Downturn > 0)
                    sb.AppendLine(ChancesLine("Spowolnienie", Downturn, ref actual));
                if (Knockdown > 0)
                    sb.AppendLine(ChancesLine("Powalenie", Knockdown, ref actual));
                if (Poison > 0)
                    sb.AppendLine(ChancesLine("Zatrucie", Poison, ref actual));
                if (Weakness > 0)
                    sb.AppendLine(ChancesLine("Osłabienie", Weakness, ref actual));
                if (Stun > 0)
                    sb.AppendLine(ChancesLine("Ogłuszenie", Stun, ref actual));
                if (Bleeding > 0)
                    sb.AppendLine(ChancesLine("Krwawienie", Bleeding, ref actual));
                if (AreaAttack > 0)
                    sb.AppendLine(ChancesLine("Atak obszarowy", AreaAttack, ref actual));
                if (Cleave > 0)
                    sb.AppendLine(ChancesLine("Przebicie", Cleave, ref actual));
                if (Fire > 0)
                    sb.AppendLine(ChancesLine("Podpalenie", Fire, ref actual));
                if (Push > 0)
                    sb.AppendLine(ChancesLine("Odepchnięcie", Push, ref actual));
                if (Terror > 0)
                    sb.AppendLine(ChancesLine("Strach", Terror, ref actual));
                if (Confusion > 0)
                    sb.AppendLine(ChancesLine("Dezorientacja", Confusion, ref actual));
                if (Hypnosis > 0)
                    sb.AppendLine(ChancesLine("Hipnoza", Hypnosis, ref actual));
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

        public Equipment(IList<string> row, string dirPath) : base(dirPath)
        {
            TypeArea = new Rectangle(79, 452, 253, 18);
            if (row.Count < 22)
                throw new ArgumentException("W wierszu znajduje się za mało kolumn by utworzyć kartę!");
            int value;
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
                if (!string.IsNullOrEmpty(row[7])) RightEffects.Add((row[7] == "1" ? "" : row[7]) + "k4"); else RightEffects.Add("");
                if (!string.IsNullOrEmpty(row[8])) RightEffects.Add((row[8] == "1" ? "" : row[8]) + "k6"); else RightEffects.Add("");
                if (!string.IsNullOrEmpty(row[9])) RightEffects.Add((row[9] == "1" ? "" : row[9]) + "k8"); else RightEffects.Add("");
                if (!string.IsNullOrEmpty(row[10])) RightEffects.Add((row[10] == "1" ? "" : row[10]) + "k10"); else RightEffects.Add("");
                if (!string.IsNullOrEmpty(row[11])) RightEffects.Add((row[11] == "1" ? "" : row[11]) + "k12"); else RightEffects.Add("");
            }
            int.TryParse(row[12], out value);
            Fatigue = value;
            int.TryParse(row[13], out value);
            FriendyFire = value;
            int.TryParse(row[14], out value);
            Hit = value;
            int.TryParse(row[15], out value);
            Downturn = value;
            int.TryParse(row[16], out value);
            Knockdown = value;
            int.TryParse(row[17], out value);
            Poison = value;
            int.TryParse(row[18], out value);
            Weakness = value;
            int.TryParse(row[19], out value);
            Stun = value;
            int.TryParse(row[20], out value);
            Bleeding = value;
            int.TryParse(row[21], out value);
            AreaAttack = value;
            int.TryParse(row[22], out value);
            Cleave = value;
            int.TryParse(row[23], out value);
            Fire = value;
            int.TryParse(row[24], out value);
            Push = value;
            int.TryParse(row[25], out value);
            Terror = value;
            int.TryParse(row[26], out value);
            Confusion = value;
            int.TryParse(row[27], out value);
            Hypnosis = value;
            int.TryParse(row[28], out value);
            Crit = value;
            int.TryParse(row[29], out value);
            Armour = value;
            Description = row[30];
            int.TryParse(row[31], out value);
            Price = value;
            if (Price == 3)
            {
                PriceLimit = 3;
                TypeArea = new Rectangle(TypeArea.X + PriceImage.Width, TypeArea.Y, TypeArea.Width - PriceImage.Width, TypeArea.Height);
            }
            string mainImagePath = dirPath + "/" + Name + ".png";
            if (!File.Exists(mainImagePath))
                mainImagePath = dirPath + "/" + Name + ".jpg";
            if (File.Exists(mainImagePath))
                MainImage = Image.FromFile(mainImagePath);
            string leftEffectsImagePath = cardsDirPath + "/left-stats.png";
            if (File.Exists(leftEffectsImagePath))
                LeftEffectsImage = Image.FromFile(leftEffectsImagePath);
        }

        public override void DrawCard(Graphics graphics)
        {
            base.DrawCard(graphics);
        }
    }
}

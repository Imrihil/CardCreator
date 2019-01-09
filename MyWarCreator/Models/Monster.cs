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
    class Monster : AttackAbilityCard
    {
        public override string FileName { get { return Name; } }
        public int Attack { get; set; }
        public Rectangle DefenceArea { get; set; } = new Rectangle(10, 95, 60, 40);
        public int HitPoints { get; set; }
        public Rectangle HitPointsArea { get; set; } = new Rectangle(10, 170, 60, 40);
        public double Level { get; set; }
        public bool IsRanged { get; set; }
        public List<ActiveAbility> ActiveAbilities { get; set; } = new List<ActiveAbility>();
        public List<PassiveAbility> PassiveAbilities { get; set; } = new List<PassiveAbility>();

        public override string DescriptionFull
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(string.Join(", ", ActiveAbilities.Select(x => x.Description)));
                if (ActiveAbilities.Any()) sb.AppendLine();
                sb.Append(string.Join(", ", PassiveAbilities.Select(x => x.Description)));
                if (PassiveAbilities.Any()) sb.AppendLine();
                sb.Append(AttackDescription());
                if (!string.IsNullOrEmpty(Description))
                {
                    sb.AppendLine();
                    sb.Append(Description);
                }
                return sb.ToString();
            }
        }

        public Monster(IList<string> row, string dirPath) : base(dirPath)
        {
            int value;
            double dvalue;
            Type = row[0];
            Name = row[1];
            int.TryParse(row[2], out value);
            Attack = value;
            LeftEffects.Add(string.Format("{0}k12", Attack));
            int.TryParse(row[3], out value);
            IsRanged = value == 1;
            int.TryParse(row[4], out value);
            Defence = value;
            int.TryParse(row[5], out value);
            HitPoints = value;
            ProcessRow(row.Skip(6).ToList());
            for (int i = 0; i < 3; ++i)
            {
                string type = row[24 + i * 3];
                string name = row[25 + i * 3];
                int.TryParse(row[26 + i * 3], out value);
                if (!string.IsNullOrEmpty(name))
                    ActiveAbilities.Add(new ActiveAbility(type, name, value));
            }
            for (int i = 33; i < 33 + 5; ++i)
            {
                string name = row[i];
                if (!string.IsNullOrEmpty(name))
                    PassiveAbilities.Add(new PassiveAbility(name));
            }
            Description = row[38];
            double.TryParse(row[39], out dvalue);
            Level = dvalue;
            if (string.IsNullOrEmpty(Type))
            {
                Type = LevelString();
            }
            else
            {
                Type = Type + " " + LevelString();
            }

            MainImage = LoadImage(dirPath, Name);
            LeftEffectsImage = LoadImage(cardsDirPath, "left-monster");
        }

        public override void DrawCard(Graphics graphics)
        {
            base.DrawCard(graphics);
            using (Font font = new Font(FontsHelper.pfc.Families.FirstOrDefault(x => x.Name.Contains("Trebuchet MS")), 20, FontStyle.Regular, GraphicsUnit.Pixel))
                graphics.DrawAdjustedString(Defence.ToString(), font, Brushes.White, DefenceArea, FontsHelper.StringFormatCentered, 6);
            using (Font font = new Font(FontsHelper.pfc.Families.FirstOrDefault(x => x.Name.Contains("Trebuchet MS")), 20, FontStyle.Regular, GraphicsUnit.Pixel))
                graphics.DrawAdjustedString(HitPoints.ToString(), font, Brushes.White, HitPointsArea, FontsHelper.StringFormatCentered, 6);
        }

        private string LevelString()
        {
            if (Level < 0.19) return "1/8";
            else if (Level < 0.28) return "1/4";
            else if (Level < 0.41) return "1/3";
            else if (Level < 0.58) return "1/2";
            else if (Level < 0.70) return "2/3";
            else if (Level < 0.81) return "3/4";
            else if (Level < 0.93) return "7/8";
            else if (Level < 1.12) return "1";
            else if (Level < 1.38) return "5/4";
            else if (Level < 1.62) return "3/2";
            else if (Level < 1.87) return "7/4";
            else if (Level < 2.75) return "5/2";
            else return ((int)Math.Round(Level)).ToString();

        }
    }

    class PassiveAbility
    {
        public string Name { get; set; }
        public virtual string Description { get { return Name; } }
        public PassiveAbility(string name)
        {
            Name = name;
        }
    }

    class ActiveAbility : PassiveAbility
    {
        public string Type { get; set; }
        public int Difficulty { get; set; }
        public override string Description { get { return (string.IsNullOrWhiteSpace(Type) ? "" : Type + ": ") + Name + (Difficulty > 0 ? (" [" + Difficulty + "+]") : ""); } }
        public ActiveAbility(string type, string name, int difficulty) : base(name)
        {
            Type = type;
            Difficulty = difficulty;
        }
    }
}

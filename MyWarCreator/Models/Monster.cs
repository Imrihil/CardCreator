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
    class Monster : Card
    {
        public override string FileName { get { return Name; } }
        public double? Attack { get; set; }
        public int Armour { get; set; }
        public int HitPoints { get; set; }
        public double Level { get; set; }
        public List<AttackAbility> AttackAbilities { get; set; } = new List<AttackAbility>();
        public List<ActiveAbility> ActiveAbilities { get; set; } = new List<ActiveAbility>();
        public List<PassiveAbility> PassiveAbilities { get; set; } = new List<PassiveAbility>();
        public string Tags { get; set; }

        public Monster(IList<string> row, string dirPath) : base(dirPath)
        {
            MainImageArea = new Rectangle(65, 20, 230, 230);

            try
            {
                double dvalue;
                int value;
                if (!string.IsNullOrEmpty(row[1]))
                    Name = row[1];
                else
                    Name = row[0];
                if ((string.IsNullOrEmpty(row[0]) && string.IsNullOrEmpty(row[1])) || (string.IsNullOrEmpty(row[5]) && string.IsNullOrEmpty(row[7])) || string.IsNullOrEmpty(row[10]) || string.IsNullOrEmpty(row[11]))
                    throw new ArgumentException($"Błędnie podane statystyki przeciwnika {Name}!");
                double.TryParse(row[4], out dvalue);
                Level = dvalue;
                double.TryParse(row[5], out dvalue);
                Attack = dvalue;
                int.TryParse(row[10], out value);
                Armour = Math.Max(value, 0);
                int.TryParse(row[11], out value);
                HitPoints = Math.Max(value, 1);
                for (int i = 0; i < 1; ++i)
                {
                    if (!(string.IsNullOrWhiteSpace(row[6 + i * 4]) && string.IsNullOrWhiteSpace(row[7 + i * 4])))
                    {
                        int.TryParse(row[9 + i * 4], out value);
                        if (string.IsNullOrEmpty(row[7 + i * 4]) && Attack.HasValue)
                            AttackAbilities.Add(new AttackAbility(row[6 + i * 4], DiceHelper.GetDices(Attack.Value), row[8 + i * 4], value));
                        else
                            AttackAbilities.Add(new AttackAbility(row[6 + i * 4], row[7 + i * 4], row[8 + i * 4], value));
                    }
                }
                for (int i = 0; i < 3; ++i)
                {
                    if (!string.IsNullOrWhiteSpace(row[13 + i * 3]))
                    {
                        int.TryParse(row[14 + i * 3], out value);
                        ActiveAbilities.Add(new ActiveAbility(row[12 + i * 3], row[13 + i * 3], value));
                    }
                }
                for (int i = 0; i < 5; ++i)
                {
                    if (!string.IsNullOrWhiteSpace(row[21 + i]))
                    {
                        int.TryParse(row[21 + i], out value);
                        PassiveAbilities.Add(new PassiveAbility(row[21 + i]));
                    }
                }
                Tags = row[26];
                if (!string.IsNullOrEmpty(Tags))
                {
                    Type = Tags + " (" + LevelString() + ")";
                }
                else
                {
                    Type = LevelString();
                }
            }
            catch (Exception)
            {
                throw new ArgumentException($"Błędnie podane statystyki przeciwnika {Name}!");
            }
            InitDescription();

            string backgroundPath = dirPath + "/background.png";
            string mainImageFramePath = dirPath + "/frame.png";
            if (File.Exists(backgroundPath))
                BackgroundImage = Image.FromFile(backgroundPath);
            InitMainImage(row[0], row[1], dirPath);
        }

        private string LevelString()
        {
            if (Level < 0.19)
                return "1/8";
            else if (Level < 0.28)
                return "1/4";
            else if (Level < 0.41)
                return "1/3";
            else if (Level < 0.58)
                return "1/2";
            else if (Level < 0.7)
                return "2/3";
            else if (Level < 0.81)
                return "3/4";
            else if (Level < 0.93)
                return "7/8";
            else if (Level < 1.12)
                return "1";
            else if (Level < 1.38)
                return "5/4";
            else if (Level < 1.62)
                return "3/2";
            else if (Level < 1.87)
                return "7/4";
            else if (Level < 2.75)
                return "5/2";
            else
                return Math.Round(Level).ToString();

        }

        private void InitMainImage(string nameAng, string namePl, string dirPath)
        {
            string mainImagePath = dirPath + "/" + namePl + ".png";
            if (!File.Exists(mainImagePath))
                mainImagePath = dirPath + "/" + namePl + ".jpg";
            if (!File.Exists(mainImagePath))
                mainImagePath = dirPath + "/" + nameAng + ".png";
            if (!File.Exists(mainImagePath))
                mainImagePath = dirPath + "/" + nameAng + ".jpg";
            if (File.Exists(mainImagePath))
                MainImage = Image.FromFile(mainImagePath);
        }

        private void InitDescription()
        {
            StringBuilder sb = new StringBuilder(string.Format("Pancerz: {0}, Wytrzymałość: {1}\n\n", Armour, HitPoints));
            foreach (var ability in AttackAbilities)
            {
                sb.AppendLine(ability.Description);
            }
            if (AttackAbilities.Any() && (ActiveAbilities.Any() || PassiveAbilities.Any()))
                sb.AppendLine("");
            foreach (var ability in ActiveAbilities)
            {
                sb.AppendLine(ability.Description);
            }
            if (ActiveAbilities.Any() && PassiveAbilities.Any())
                sb.AppendLine("");
            sb.Append(string.Join(", ", PassiveAbilities.Select(x => x.Description.ToString())));
            Description = sb.ToString();
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
        public override string Description { get { return (string.IsNullOrWhiteSpace(Type) ? "" : Type + ": ") + Name + (Difficulty > 0 ? (" [" + Difficulty + "]") : ""); } }
        public ActiveAbility(string type, string name, int difficulty) : base(name)
        {
            Type = type;
            Difficulty = difficulty;
        }
    }

    class AttackAbility : ActiveAbility
    {
        public string Dmg { get; set; }
        public override string Description { get { return (string.IsNullOrWhiteSpace(Type) ? "" : Type + ": ") + Dmg + (string.IsNullOrEmpty(Name) ? "" : (" + " + Name + (Difficulty > 0 ? (" [" + Difficulty + "]") : ""))); } }
        public AttackAbility(string type, string dmg, string name, int difficulty) : base(type, name, difficulty)
        {
            Dmg = dmg;
        }
    }
}

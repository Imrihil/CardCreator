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
        public string SpecialAttack { get; set; }
        public int Armour { get; set; }
        public int HitPoints { get; set; }
        public string SpecialQualities { get; set; }

        public Monster(MonsterData monster, string dirPath) : base(dirPath)
        {
            MainImageArea = new Rectangle(65, 20, 230, 230);

            Name = monster.Name;
            Type = "Poziom wyzwania: " + monster.ChallengeRating;
            if (monster.Str.HasValue)
                Attack = (double)(monster.Str - 6) / 2;
            if (!string.IsNullOrEmpty(monster.FirstAttack))
                if (Attack == null)
                    Attack = DiceHelper.GetAverage(monster.FirstAttack);
                else
                    Attack += DiceHelper.GetAverage(monster.FirstAttack);
            if (Attack != null && Attack < 0.5)
                Attack = 0.5;
            if (!string.IsNullOrEmpty(monster.FirstAttack))
            {
                if (DiceHelper.HasDice(monster.FirstAttack))
                {
                    int attackIdx = monster.FirstAttack.IndexOf(" ");
                    if (attackIdx >= 0)
                        SpecialAttack = monster.FirstAttack.Substring(attackIdx + 1);
                }
                else
                {
                    SpecialAttack = monster.FirstAttack;
                }
            }
            Armour = Convert.ToInt32(monster.ArmourClass * 0.5) - 5;
            if (Armour < 0) Armour = 0;
            HitPoints = Convert.ToInt32(monster.HitDice * 0.2);
            if (HitPoints < 1) HitPoints = 1;
            SpecialQualities = monster.SpecialQualities;

            string backgroundPath = dirPath + "/background.png";
            string mainImageFramePath = dirPath + "/frame.png";
            if (File.Exists(backgroundPath))
                BackgroundImage = Image.FromFile(backgroundPath);
            if (File.Exists(mainImageFramePath))
                MainImageFrame = Image.FromFile(mainImageFramePath);
            string mainImagePath = dirPath + "/" + Name + ".png";
            if (!File.Exists(mainImagePath))
                mainImagePath = dirPath + "/" + Name + ".jpg";
            if (!File.Exists(mainImagePath))
                mainImagePath = monster.ImagePath;
            if (File.Exists(mainImagePath))
                MainImage = Image.FromFile(mainImagePath);

            Description = string.Format("Atak: {0}{1}\n\nPancerz: {2}\nWytrzymałość: {3}\n\n{4}",
                Attack.HasValue ? DiceHelper.GetDices(Attack.Value) : "-",
                string.IsNullOrEmpty(SpecialAttack) ? "" : " " + SpecialAttack,
                Armour, HitPoints, SpecialQualities);
        }

        public Monster(IList<string> row, string dirPath) : base(dirPath)
        {
            MainImageArea = new Rectangle(65, 20, 230, 230);

            try
            {
                int value;
                if (!string.IsNullOrEmpty(row[1]))
                    Name = row[1];
                else
                    Name = row[0];
                if ((string.IsNullOrEmpty(row[0]) && string.IsNullOrEmpty(row[1])) || string.IsNullOrEmpty(row[3]) || string.IsNullOrEmpty(row[5]) || string.IsNullOrEmpty(row[6]))
                    throw new ArgumentException($"Błędnie podane statystyki przeciwnika {Name}!");
                Type = "Poziom wyzwania: " + row[2];
                if (row[3].Contains("d") || row[3].Contains("k"))
                {
                    Attack = DiceHelper.GetAverage(row[3]);
                }
                else
                {
                    int.TryParse(row[3], out value);
                    Attack = Math.Max(value, 1);
                }
                SpecialAttack = row[4];
                int.TryParse(row[5], out value);
                Armour = value;
                int.TryParse(row[6], out value);
                HitPoints = Math.Max(value, 1);
                SpecialQualities = row[7];
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
            if (File.Exists(mainImageFramePath))
                MainImageFrame = Image.FromFile(mainImageFramePath);
            InitMainImage(row[0], row[1], dirPath);
        }

        internal void Update(IList<string> row, string dirPath)
        {
            if (!string.IsNullOrEmpty(row[1]))
                Name = row[1];
            if (!string.IsNullOrEmpty(row[2]))
                Type = "Poziom wyzwania: " + row[2];
            if (!string.IsNullOrEmpty(row[3]))
                Attack = int.Parse(row[3]);
            if (!string.IsNullOrEmpty(row[4]))
                SpecialAttack = row[4];
            if (!string.IsNullOrEmpty(row[5]))
                Armour = int.Parse(row[5]);
            if (!string.IsNullOrEmpty(row[6]))
                HitPoints = int.Parse(row[6]);
            if (!string.IsNullOrEmpty(row[7]))
                SpecialQualities = row[7];

            InitMainImage(row[0], row[1], dirPath);
            InitDescription();
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
            Description = string.Format("Atak: {0}{1}\n\nPancerz: {2}\nWytrzymałość: {3}\n\n{4}",
                Attack.HasValue ? DiceHelper.GetDices(Attack.Value) : "-",
                string.IsNullOrEmpty(SpecialAttack) ? "" : " " + SpecialAttack,
                Armour, HitPoints, SpecialQualities);
        }
    }
}

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
            if (Attack != null && !string.IsNullOrEmpty(monster.FirstAttack))
                Attack += DiceHelper.GetAverage(monster.FirstAttack);
            if (Attack != null && Attack < 0.5)
                Attack = 0.5;
            if (!string.IsNullOrEmpty(monster.FirstAttack))
            {
                int attackIdx = monster.FirstAttack.IndexOf(" ");
                if (attackIdx >= 0)
                    SpecialAttack = monster.FirstAttack.Substring(attackIdx + 1);
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
            string mainImagePath = dirPath + "/" + Name + ".jpg";
            if (!File.Exists(mainImagePath))
                mainImagePath = monster.ImagePath;
            if (File.Exists(mainImagePath))
                MainImage = Image.FromFile(mainImagePath);

            Description = string.Format("Atak: {0}{1}\n\nPancerz: {2}\nWytrzymałość: {3}\n\n{4}",
                Attack.HasValue ? DiceHelper.GetDices(Attack.Value) : "-",
                string.IsNullOrEmpty(SpecialAttack) ? "" : " " + SpecialAttack,
                Armour, HitPoints, SpecialQualities);
        }
    }
}

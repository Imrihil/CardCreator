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
        public double? Attack { get; }
        public string SpecialAttack { get; }
        public int Armour { get; }
        public int HitPoints { get; }

        public Monster(MonsterData monster, string dirPath) : base(dirPath)
        {
            MainImageArea = new Rectangle(65, 20, 230, 230);
            Name = monster.Stats["Name"];
            Type = "Poziom wyzwania: " + monster.Stats["Challenge Rating"];
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
            string armourString = monster.Stats["Armor Class"];
            if (!string.IsNullOrEmpty(armourString))
            {
                int armourIdx = armourString.IndexOf(" ");
                if (armourIdx >= 0)
                    armourString = armourString.Substring(0, armourIdx);
                int armour;
                int.TryParse(armourString, out armour);
                Armour = Convert.ToInt32(armour * 0.5) - 5;
            }
            string healthString = monster.Stats["Hit Dice"];
            if (!string.IsNullOrEmpty(healthString))
            {
                int healthIdx = healthString.IndexOf("(");
                if (healthIdx >= 0)
                {
                    int healthIdx2 = healthString.IndexOf(" ", healthIdx);
                    if (healthIdx2 >= 0)
                        healthString = healthString.Substring(healthIdx + 1, healthIdx2 - healthIdx - 1);
                    int health;
                    int.TryParse(healthString, out health);
                    HitPoints = Convert.ToInt32(health * 0.2);
                }
            }

            string backgroundPath = dirPath + "/background.png";
            string mainImageFramePath = dirPath + "/frame.png";
            if (File.Exists(backgroundPath))
                BackgroundImage = Image.FromFile(backgroundPath);
            if (File.Exists(mainImageFramePath))
                MainImageFrame = Image.FromFile(mainImageFramePath);
            string mainImagePath = monster.ImagePath;
            if (File.Exists(mainImagePath))
                MainImage = Image.FromFile(mainImagePath);

            Description = string.Format("Atak: {0}{1}\n\nPancerz: {2}\nWytrzymałość: {3}",
                Attack.HasValue ? DiceHelper.GetDices(Attack.Value) : "-",
                string.IsNullOrEmpty(SpecialAttack) ? "" : " " + SpecialAttack,
                Armour, HitPoints);
        }
    }
}

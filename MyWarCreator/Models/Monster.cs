using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MyWarCreator.Extensions;
using MyWarCreator.Helpers;

namespace MyWarCreator.Models
{
    public sealed class Monster : AttackAbilityCard
    {
        protected override string FileName => Name;
        private int Attack { get; }
        private Rectangle DefendArea { get; } = new Rectangle(10, 95, 60, 40);
        private int HitPoints { get; }
        private Rectangle HitPointsArea { get; } = new Rectangle(10, 170, 60, 40);
        private double Level { get; }
        private List<ActiveAbility> ActiveAbilities { get; } = new List<ActiveAbility>();
        private List<PassiveAbility> PassiveAbilities { get; } = new List<PassiveAbility>();

        protected override string DescriptionFull
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append(string.Join(", ", ActiveAbilities.Select(x => x.Description)));
                if (ActiveAbilities.Any()) sb.AppendLine();
                sb.Append(string.Join(", ", PassiveAbilities.Select(x => x.Description)));
                if (PassiveAbilities.Any()) sb.AppendLine();
                sb.Append(AttackDescription());
                if (string.IsNullOrEmpty(Description)) return sb.ToString();

                sb.AppendLine();
                sb.Append(Description);
                return sb.ToString();
            }
        }

        public Monster(IList<string> row, string dirPath) : base(dirPath)
        {
            Type = row[0];
            Name = row[1];
            int.TryParse(row[2], out var value);
            Attack = value;
            LeftEffects.Add($"{Attack}k12");
            int.TryParse(row[3], out value);
            int.TryParse(row[4], out value);
            Defence = value;
            int.TryParse(row[5], out value);
            HitPoints = value;
            ProcessRow(row.Skip(6).ToList());
            for (var i = 0; i < 3; ++i)
            {
                var type = row[24 + i * 3];
                var name = row[25 + i * 3];
                int.TryParse(row[26 + i * 3], out value);
                if (!string.IsNullOrEmpty(name))
                    ActiveAbilities.Add(new ActiveAbility(type, name, value));
            }
            for (var i = 33; i < 33 + 5; ++i)
            {
                var name = row[i];
                if (!string.IsNullOrEmpty(name))
                    PassiveAbilities.Add(new PassiveAbility(name));
            }
            Description = row[38];
            double.TryParse(row[39], out var doubleValue);
            Level = doubleValue;
            if (string.IsNullOrEmpty(Type))
            {
                Type = LevelString();
            }
            else
            {
                Type = Type + " " + LevelString();
            }

            MainImage = LoadImage(dirPath, Name);
            LeftEffectsImage = LoadImage(CardsDirPath, "left-monster");
        }

        protected override void DrawCard(Graphics graphics)
        {
            base.DrawCard(graphics);
            using (var font = new Font(FontTrebuchetMs, 20, FontStyle.Regular, GraphicsUnit.Pixel))
                graphics.DrawAdjustedString(Defence.ToString(), font, Brushes.White, DefendArea, FontsHelper.StringFormatCentered, 6);
            using (var font = new Font(FontTrebuchetMs, 20, FontStyle.Regular, GraphicsUnit.Pixel))
                graphics.DrawAdjustedString(HitPoints.ToString(), font, Brushes.White, HitPointsArea, FontsHelper.StringFormatCentered, 6);
        }

        private string LevelString()
        {
            if (Level < 0.19) return "1/8";
            if (Level < 0.28) return "1/4";
            if (Level < 0.41) return "1/3";
            if (Level < 0.58) return "1/2";
            if (Level < 0.70) return "2/3";
            if (Level < 0.81) return "3/4";
            if (Level < 0.93) return "7/8";
            if (Level < 1.12) return "1";
            if (Level < 1.38) return "5/4";
            if (Level < 1.62) return "3/2";
            if (Level < 1.87) return "7/4";
            if (Level < 2.75) return "5/2";
            return ((int)Math.Round(Level)).ToString();

        }
    }

    public class PassiveAbility
    {
        protected string Name { get; }
        public virtual string Description => Name;

        public PassiveAbility(string name)
        {
            Name = name;
        }
    }

    public class ActiveAbility : PassiveAbility
    {
        private string Type { get; }
        private int Difficulty { get; }
        public override string Description => (string.IsNullOrWhiteSpace(Type) ? "" : Type + ": ") + Name + (Difficulty > 0 ? (" [" + Difficulty + "+]") : "");

        public ActiveAbility(string type, string name, int difficulty) : base(name)
        {
            Type = type;
            Difficulty = difficulty;
        }
    }
}

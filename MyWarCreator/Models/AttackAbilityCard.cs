using System.Collections.Generic;
using System.Text;

namespace MyWarCreator.Models
{
    public class AttackAbilityCard : Card
    {
        private int Fatigue { get; set; }
        private int FriendlyFire { get; set; }
        private int Hit { get; set; }
        private int Downturn { get; set; }
        private int Weakness { get; set; }
        private int Poison { get; set; }
        private int Bleeding { get; set; }
        private int AreaAttack { get; set; }
        private int Blinding { get; set; }
        private int Cleave { get; set; }
        private int Fire { get; set; }
        private int Calm { get; set; }
        private int Stun { get; set; }
        private int Knockdown { get; set; }
        private int Hypnosis { get; set; }
        private int Push { get; set; }
        private int Terror { get; set; }
        private int Crit { get; set; }
        protected int Defence { get; set; }

        protected AttackAbilityCard(string dirPath) : base(dirPath)
        {
        }

        protected string AttackDescription()
        {
            var sb = new StringBuilder();
            var actual = 1;
            if (Fatigue > 0)
                sb.AppendLine(ChancesLine("Zmęczenie", Fatigue, ref actual));
            if (FriendlyFire > 0)
                sb.AppendLine(ChancesLine("Swój", FriendlyFire, ref actual));
            if (Hit > 0)
                ChancesLine("Trafienie", Hit, ref actual);
            if (Downturn > 0)
                sb.AppendLine(ChancesLine("Spowolnienie", Downturn, ref actual));
            if (Weakness > 0)
                sb.AppendLine(ChancesLine("Osłabienie", Weakness, ref actual));
            if (Poison > 0)
                sb.AppendLine(ChancesLine("Zatrucie", Poison, ref actual));
            if (Bleeding > 0)
                sb.AppendLine(ChancesLine("Krwawienie", Bleeding, ref actual));
            if (AreaAttack > 0)
                sb.AppendLine(ChancesLine("Atak obszarowy", AreaAttack, ref actual));
            if (Blinding > 0)
                sb.AppendLine(ChancesLine("Oślepienie", Blinding, ref actual));
            if (Cleave > 0)
                sb.AppendLine(ChancesLine("Przebicie", Cleave, ref actual));
            if (Fire > 0)
                sb.AppendLine(ChancesLine("Podpalenie", Fire, ref actual));
            if (Calm > 0)
                sb.AppendLine(ChancesLine("Wyciszenie", Calm, ref actual));
            if (Stun > 0)
                sb.AppendLine(ChancesLine("Ogłuszenie", Stun, ref actual));
            if (Knockdown > 0)
                sb.AppendLine(ChancesLine("Powalenie", Knockdown, ref actual));
            if (Hypnosis > 0)
                sb.AppendLine(ChancesLine("Hipnoza", Hypnosis, ref actual));
            if (Push > 0)
                sb.AppendLine(ChancesLine("Odepchnięcie", Push, ref actual));
            if (Terror > 0)
                sb.AppendLine(ChancesLine("Strach", Terror, ref actual));
            if (Crit > 0)
                sb.Append(ChancesLine("Krytyk", Crit, ref actual));
            return sb.ToString();
        }

        protected void ProcessRow(IList<string> row)
        {
            int.TryParse(row[0], out var value);
            Fatigue = value;
            int.TryParse(row[1], out value);
            FriendlyFire = value;
            int.TryParse(row[2], out value);
            Hit = value;
            int.TryParse(row[3], out value);
            Downturn = value;
            int.TryParse(row[4], out value);
            Weakness = value;
            int.TryParse(row[5], out value);
            Poison = value;
            int.TryParse(row[6], out value);
            Bleeding = value;
            int.TryParse(row[7], out value);
            AreaAttack = value;
            int.TryParse(row[8], out value);
            Blinding = value;
            int.TryParse(row[9], out value);
            Cleave = value;
            int.TryParse(row[10], out value);
            Fire = value;
            int.TryParse(row[11], out value);
            Calm = value;
            int.TryParse(row[12], out value);
            Stun = value;
            int.TryParse(row[13], out value);
            Knockdown = value;
            int.TryParse(row[14], out value);
            Hypnosis = value;
            int.TryParse(row[15], out value);
            Push = value;
            int.TryParse(row[16], out value);
            Terror = value;
            int.TryParse(row[17], out value);
            Crit = value;
        }

        private string ChancesLine(string name, int hitChance, ref int actual)
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

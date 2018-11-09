using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWarCreator.Models
{
    class AttackAbilityCard : Card
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
        public int Defence { get; set; }

        public AttackAbilityCard(string dirPath) : base(dirPath)
        {
        }

        protected string AttackDescription()
        {
            StringBuilder sb = new StringBuilder();
            int actual = 1;
            if (Fatigue > 0)
                sb.AppendLine(ChancesLine("Zmęczenie", Fatigue, ref actual));
            if (FriendyFire > 0)
                sb.AppendLine(ChancesLine("Swój", FriendyFire, ref actual));
            if (Hit > 0)
                ChancesLine("Trafienie", Hit, ref actual);
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
                sb.Append(ChancesLine("Krytyk", Crit, ref actual));
            return sb.ToString();
        }

        protected void ProcessRow(IList<string> row)
        {
            int value;
            int.TryParse(row[0], out value);
            Fatigue = value;
            int.TryParse(row[1], out value);
            FriendyFire = value;
            int.TryParse(row[2], out value);
            Hit = value;
            int.TryParse(row[3], out value);
            Downturn = value;
            int.TryParse(row[4], out value);
            Knockdown = value;
            int.TryParse(row[5], out value);
            Poison = value;
            int.TryParse(row[6], out value);
            Weakness = value;
            int.TryParse(row[7], out value);
            Stun = value;
            int.TryParse(row[8], out value);
            Bleeding = value;
            int.TryParse(row[9], out value);
            AreaAttack = value;
            int.TryParse(row[10], out value);
            Cleave = value;
            int.TryParse(row[11], out value);
            Fire = value;
            int.TryParse(row[12], out value);
            Push = value;
            int.TryParse(row[13], out value);
            Terror = value;
            int.TryParse(row[14], out value);
            Confusion = value;
            int.TryParse(row[15], out value);
            Hypnosis = value;
            int.TryParse(row[16], out value);
            Crit = value;
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
    }
}

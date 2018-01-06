using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyWarCreator.Helpers
{
    class MonsterData
    {
        public Dictionary<string, string> Stats { get; }
        public string ImagePath { get; }
        public int? Str { get; private set; }
        public int? Dex { get; private set; }
        public int? Con { get; private set; }
        public int? Int { get; private set; }
        public int? Wis { get; private set; }
        public int? Cha { get; private set; }
        public string FirstAttack { get; private set; }
        public string SecondAttack { get; private set; }

        public MonsterData(List<List<string>> table, string defaultName, string imagePath, int colId)
        {
            Stats = new Dictionary<string, string>();
            for (int i = 0; i < table.Count; ++i)
            {
                var statName = table[i][0];
                statName = statName.Trim(':');
                var statVal = table[i][colId];
                if (i == 0 && string.IsNullOrEmpty(statName)) statName = "Name";
                Stats.Add(statName, statVal);
            }
            if(!Stats.ContainsKey("Name"))
            {
                Stats.Add("Name", defaultName);
            }
            ImagePath = imagePath;
            InitAbilities();
            InitAttacks();
        }

        private void InitAbilities()
        {
            var abilities = Stats["Abilities"];
            if (abilities != null)
            {
                Str = GetAbility("Str", abilities);
                Dex = GetAbility("Dex", abilities);
                Con = GetAbility("Con", abilities);
                Int = GetAbility("Int", abilities);
                Wis = GetAbility("Wis", abilities);
                Cha = GetAbility("Cha", abilities);
            }
        }

        private int? GetAbility(string abilityName, string abilities)
        {
            int idxStart = abilities.IndexOf(abilityName);
            int idxEnd = abilities.IndexOf(",", idxStart);
            if (idxEnd == -1) idxEnd = abilities.Length;
            int abilityValue;
            if (!int.TryParse(abilities.Substring(idxStart + 4, idxEnd - idxStart - 4), out abilityValue))
            {
                return null;
            }
            return abilityValue;
        }

        private void InitAttacks()
        {
            var fullAttack = Stats["Full Attack"];
            if (fullAttack != null)
            {
                int idxStart = fullAttack.IndexOf("(");
                if (idxStart > -1)
                {
                    int idxEnd = fullAttack.IndexOf(")", idxStart);
                    if (idxEnd > -1)
                        FirstAttack = fullAttack.Substring(idxStart + 1, idxEnd - idxStart - 1);
                    idxStart = fullAttack.IndexOf("(", idxEnd);
                    if (idxStart > -1)
                    {
                        idxEnd = fullAttack.IndexOf(")", idxStart);
                        if (idxEnd > -1)
                            FirstAttack = fullAttack.Substring(idxStart + 1, idxEnd - idxStart - 1);
                    }
                }
            }
        }
    }
}

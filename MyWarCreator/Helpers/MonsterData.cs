using System;
using System.Collections.Generic;

namespace MyWarCreator.Helpers
{
    public class MonsterData
    {
        private Dictionary<string, string> Stats { get; }
        public string Name { get; private set; }
        public int? Str { get; private set; }
        public int? Dex { get; private set; }
        public int? Con { get; private set; }
        public int? Int { get; private set; }
        public int? Wis { get; private set; }
        public int? Cha { get; private set; }
        public int? ArmourClass { get; private set; }
        public int? HitDice { get; private set; }
        public string FirstAttack { get; private set; }
        public string SecondAttack { get; private set; }
        public string ChallengeRating { get; private set; }
        public string SpecialQualities { get; private set; }
        public string ImagePath { get; }
        public static IList<string> Headers =>
            new[] {
                "Name",
                "Str",
                "Dex",
                "Con",
                "Int",
                "Wis",
                "Cha",
                "ArmourClass",
                "HitDice",
                "FirstAttack",
                "SecondAttack",
                "ChallengeRating",
                "SpecialQualities",
                "ImagePath"
            };

        public IList<string> Row =>
            new[] {
                Name,
                Str.ToString(),
                Dex.ToString(),
                Con.ToString(),
                Int.ToString(),
                Wis.ToString(),
                Cha.ToString(),
                ArmourClass.ToString(),
                HitDice.ToString(),
                FirstAttack,
                SecondAttack,
                ChallengeRating,
                SpecialQualities,
                ImagePath
            };

        public MonsterData(List<List<string>> table, string defaultName, string imagePath, int colId)
        {
            Stats = new Dictionary<string, string>();
            for (var i = 0; i < table.Count; ++i)
            {
                var statName = table[i][0];
                statName = statName.Trim(':');
                var statVal = table[i][colId];
                if (i == 0 && string.IsNullOrEmpty(statName)) statName = "Name";
                Stats.Add(statName, statVal);
            }
            if (!Stats.ContainsKey("Name"))
            {
                Stats.Add("Name", defaultName);
            }
            ImagePath = imagePath;
            InitAbilities();
            InitAttacks();
            InitStats();
        }

        public MonsterData(IList<string> row)
        {
            Name = row[0];
            if (!string.IsNullOrEmpty(row[1]))
                Str = int.Parse(row[1]);
            if (!string.IsNullOrEmpty(row[2]))
                Dex = int.Parse(row[2]);
            if (!string.IsNullOrEmpty(row[3]))
                Con = int.Parse(row[3]);
            if (!string.IsNullOrEmpty(row[4]))
                Int = int.Parse(row[4]);
            if (!string.IsNullOrEmpty(row[5]))
                Wis = int.Parse(row[5]);
            if (!string.IsNullOrEmpty(row[6]))
                Cha = int.Parse(row[6]);
            if (!string.IsNullOrEmpty(row[7]))
                ArmourClass = int.Parse(row[7]);
            if (!string.IsNullOrEmpty(row[8]))
                HitDice = int.Parse(row[8]);
            FirstAttack = row[9];
            SecondAttack = row[10];
            ChallengeRating = row[11];
            SpecialQualities = row[12];
            ImagePath = row[13];
        }

        private void InitStats()
        {
            Name = Stats["Name"];
            ChallengeRating = Stats["Challenge Rating"].ToLower();
            if (ChallengeRating.Contains("frac"))
            {
                var idx = ChallengeRating.IndexOf("frac", StringComparison.InvariantCultureIgnoreCase);
                ChallengeRating = ChallengeRating[idx + 4] + "/" + ChallengeRating[idx + 5];
            }
            var armourString = Stats["Armor Class"];
            if (!string.IsNullOrEmpty(armourString))
            {
                var armourIdx = armourString.IndexOf(" ", StringComparison.InvariantCultureIgnoreCase);
                if (armourIdx >= 0)
                    armourString = armourString.Substring(0, armourIdx);
                int.TryParse(armourString, out var armour);
                ArmourClass = armour;
            }
            var healthString = Stats["Hit Dice"];
            if (!string.IsNullOrEmpty(healthString))
            {
                var healthIdx = healthString.IndexOf("(", StringComparison.InvariantCultureIgnoreCase);
                if (healthIdx >= 0)
                {
                    var healthIdx2 = healthString.IndexOf(" ", healthIdx, StringComparison.InvariantCultureIgnoreCase);
                    if (healthIdx2 >= 0)
                        healthString = healthString.Substring(healthIdx + 1, healthIdx2 - healthIdx - 1);
                    int.TryParse(healthString, out var health);
                    HitDice = health;
                }
            }
            SpecialQualities = Stats["Special Qualities"];
        }

        private void InitAbilities()
        {
            var abilities = Stats["Abilities"];
            if (abilities == null) return;

            Str = GetAbility("Str", abilities);
            Dex = GetAbility("Dex", abilities);
            Con = GetAbility("Con", abilities);
            Int = GetAbility("Int", abilities);
            Wis = GetAbility("Wis", abilities);
            Cha = GetAbility("Cha", abilities);
        }

        private int? GetAbility(string abilityName, string abilities)
        {
            var idxStart = abilities.IndexOf(abilityName, StringComparison.InvariantCultureIgnoreCase);
            var idxEnd = abilities.IndexOf(",", idxStart, StringComparison.InvariantCultureIgnoreCase);
            if (idxEnd == -1) idxEnd = abilities.Length;
            if (!int.TryParse(abilities.Substring(idxStart + 4, idxEnd - idxStart - 4), out var abilityValue))
            {
                return null;
            }
            return abilityValue;
        }

        private void InitAttacks()
        {
            var fullAttack = Stats["Full Attack"];
            if (fullAttack == null) return;

            var idxStart = fullAttack.IndexOf("(", StringComparison.InvariantCultureIgnoreCase);
            if (idxStart <= -1) return;

            var idxEnd = fullAttack.IndexOf(")", idxStart, StringComparison.InvariantCultureIgnoreCase);
            if (idxEnd > -1)
                FirstAttack = fullAttack.Substring(idxStart + 1, idxEnd - idxStart - 1);
            idxStart = fullAttack.IndexOf("(", idxEnd, StringComparison.InvariantCultureIgnoreCase);
            if (idxStart <= -1) return;

            idxEnd = fullAttack.IndexOf(")", idxStart, StringComparison.InvariantCultureIgnoreCase);
            if (idxEnd > -1)
                SecondAttack = fullAttack.Substring(idxStart + 1, idxEnd - idxStart - 1);
        }
    }
}

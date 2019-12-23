using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CardCreator.Helpers
{
    public class DiceHelper
    {
        private static readonly Dice D1 = new Dice(1, 1);
        private static readonly Dice D2 = new Dice(1, 2);
        private static readonly Dice D3 = new Dice(1, 3);
        private static readonly Dice D4 = new Dice(1, 4);
        private static readonly Dice D6 = new Dice(1, 6);
        private static readonly Dice D8 = new Dice(1, 8);
        private static readonly Dice D10 = new Dice(1, 10);
        private static readonly Dice D12 = new Dice(1, 12);
        private static readonly Dice D20 = new Dice(1, 20);
        private static readonly Dice D100 = new Dice(1, 100);
        private static readonly List<Dice> Dices = new List<Dice> { D1, D2, D3, D4, D6, D8, D10, D12, D20, D100 };
        private static readonly List<DicesSet> DicesForSum = new List<DicesSet>();

        static DiceHelper()
        {
            DicesForSum.Add(new DicesSet()); // 0
            DicesForSum.Add(new DicesSet { D1 }); // 0.5
            DicesForSum.Add(new DicesSet { D1 }); // 1
            DicesForSum.Add(new DicesSet { D2 }); // 1.5
            DicesForSum.Add(new DicesSet { D3 }); // 2
            DicesForSum.Add(new DicesSet { D4 }); // 2.5
            DicesForSum.Add(new DicesSet { D4 }); // 3
            DicesForSum.Add(new DicesSet { D6 }); // 3.5
            DicesForSum.Add(new DicesSet { D6 }); // 4
            DicesForSum.Add(new DicesSet { D8 }); // 4.5
            DicesForSum.Add(new DicesSet { { D4, 2 } }); // 5
            DicesForSum.Add(new DicesSet { D10 }); // 5.5
            DicesForSum.Add(new DicesSet { D4, D6 }); // 6
            DicesForSum.Add(new DicesSet { D12 }); // 6.5
            DicesForSum.Add(new DicesSet { { D6, 2 } }); // 7
            DicesForSum.Add(new DicesSet { { D6, 2 } }); // 7.5
            DicesForSum.Add(new DicesSet { D6, D8 }); // 8
            DicesForSum.Add(new DicesSet { D6, D8 }); // 8.5
            DicesForSum.Add(new DicesSet { { D8, 2 } }); // 9
            DicesForSum.Add(new DicesSet { { D8, 2 } }); // 9.5
            DicesForSum.Add(new DicesSet { D8, D10 }); // 10
            DicesForSum.Add(new DicesSet { D20 }); // 10.5
            DicesForSum.Add(new DicesSet { { D10, 2 } }); // 11
            DicesForSum.Add(new DicesSet { { D10, 2 } }); // 11.5
            DicesForSum.Add(new DicesSet { D10, D12 }); // 12
            DicesForSum.Add(new DicesSet { D10, D12 }); // 12.5
            DicesForSum.Add(new DicesSet { { D12, 2 } }); // 13
            DicesForSum.Add(new DicesSet { { D12, 2 } }); // 13.5
            DicesForSum.Add(new DicesSet { D6, D20 }); // 14
            DicesForSum.Add(new DicesSet { { D8, 2 }, D10 }); // 14.5
            DicesForSum.Add(new DicesSet { D8, D20 }); // 15
            DicesForSum.Add(new DicesSet { D8, { D10, 2 } }); // 15.5
            DicesForSum.Add(new DicesSet { D10, D20 }); // 16
            DicesForSum.Add(new DicesSet { { D10, 3 } }); // 16.5
            DicesForSum.Add(new DicesSet { D12, D20 }); // 17
            DicesForSum.Add(new DicesSet { { D10, 2 }, D12 }); // 17.5
            DicesForSum.Add(new DicesSet { { D8, 4 } }); // 18
            DicesForSum.Add(new DicesSet { D10, { D12, 2 } }); // 18.5
            DicesForSum.Add(new DicesSet { { D8, 3 }, D10 }); // 19
            DicesForSum.Add(new DicesSet { { D12, 3 } }); // 19.5
            DicesForSum.Add(new DicesSet { { D8, 2 }, { D10, 2 } }); // 20
            DicesForSum.Add(new DicesSet { { D8, 2 }, { D10, 2 } }); // 20.5
            DicesForSum.Add(new DicesSet { { D20, 2 } }); // 21
        }

        public static string GetDices(double average)
        {
            if (average < 0)
            {
                throw new ArgumentException("The average cannot be negative!");
            }
            var sum = Convert.ToInt32(2 * average);
            if (sum < DicesForSum.Count)
            {
                return DicesForSum[sum].ToString();
            }

            if (sum == DicesForSum.Count)
            {
                return DicesForSum[DicesForSum.Count - 1].ToString();
            }

            return DicesForSum[DicesForSum.Count - 1] + "+" + (sum + 1 - DicesForSum.Count) / 2;
        }

        public static bool HasDice(string dice)
        {
            return Regex.IsMatch(dice, @"\d*[kd]\d+");
        }

        public static bool IsDice(string dice)
        {
            return Regex.IsMatch(dice, @"^\d*[kd]\d+$");
        }

        private static bool IsDiceOrNumber(string dice)
        {
            return Regex.IsMatch(dice, @"^\d*[kd]\d+$") || double.TryParse(dice, out _);
        }

        public static double GetAverage(string dices)
        {
            var idx = dices.ToLower().IndexOf(" plus ", StringComparison.InvariantCultureIgnoreCase);
            if (idx >= 0)
            {
                return GetAverageDices(dices.Substring(0, idx)) + GetAverageDices(dices.Substring(idx + 6));
            }
            return GetAverageDices(dices);
        }

        private static double GetAverageDices(string dices)
        {
            double result = 0;
            var idx = dices.IndexOf(" ", StringComparison.InvariantCultureIgnoreCase);
            if (idx >= 0)
                dices = dices.Substring(0, idx);
            idx = dices.IndexOf("/", StringComparison.InvariantCultureIgnoreCase);
            if (idx >= 0)
                dices = dices.Substring(0, idx);
            idx = dices.IndexOf("*", StringComparison.InvariantCultureIgnoreCase);
            if (idx >= 0)
                dices = dices.Substring(0, idx);
            dices = dices.Replace("d", "k");
            var minus = dices.Contains("-");
            var dicesStrings = Regex.Split(dices, @"[+-]");
            for (var i = 0; i < dicesStrings.Length; ++i)
            {
                if (!IsDiceOrNumber(dicesStrings[i])) continue;

                idx = dicesStrings[i].IndexOf("k", StringComparison.InvariantCultureIgnoreCase);
                string numberString;
                string diceName;
                if (idx >= 0)
                {
                    numberString = dicesStrings[i].Substring(0, idx);
                    diceName = dicesStrings[i].Substring(idx);
                }
                else
                {
                    numberString = dicesStrings[i];
                    diceName = "1";
                }
                int.TryParse(numberString, out var number);
                if (minus && i == dicesStrings.Length - 1)
                    result -= number * (Dices.FirstOrDefault(x => x.Name == diceName)?.Average ?? 0);
                else
                    result += number * (Dices.FirstOrDefault(x => x.Name == diceName)?.Average ?? 0);
            }
            return result;
        }
    }

    public class DicesSet : Dictionary<Dice, int>
    {
        public int CountDices { get { return this.Sum(x => x.Value); } }
        public int SumDices { get { return this.Sum(x => x.Value * x.Key.Sum); } }
        public void Add(Dice key)
        {
            if (ContainsKey(key))
                this[key]++;
            else
                Add(key, 1);
        }
        public void Subtract(Dice key)
        {
            if (ContainsKey(key))
            {
                this[key]--;
                if (this[key] == 0)
                    Remove(key);
            }
            else
            {
                throw new ArgumentException("The set does not contain given key!");
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var p in this.OrderByDescending(x => x.Key))
            {
                if (p.Value <= 0) continue;

                if (p.Value > 1)
                    sb.Append(p.Value);
                sb.Append(p.Key);
                sb.Append("+");
            }
            return sb.Length == 0
                ? "0" : sb.ToString(0, sb.Length - 1);
        }
    }

    public class Dice : IComparable<Dice>
    {
        public string Name { get; }
        private int MinValue { get; }
        private int MaxValue { get; }
        public int Sum { get; }
        public double Average { get; }

        public Dice(int minValue, int maxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            Sum = MinValue + MaxValue;
            // ReSharper disable once PossibleLossOfFraction
            Average = Sum / 2;
            Name = MaxValue > 1 ? "k" + MaxValue : "1";
        }

        public override string ToString()
        {
            return Name;
        }

        public int CompareTo(Dice obj)
        {
            return MaxValue.CompareTo(obj.MaxValue);
        }
    }
}

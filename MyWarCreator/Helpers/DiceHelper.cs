using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyWarCreator.Helpers
{
    class DiceHelper
    {
        public static readonly Dice d1 = new Dice(1, 1);
        public static readonly Dice d2 = new Dice(1, 2);
        public static readonly Dice d3 = new Dice(1, 3);
        public static readonly Dice d4 = new Dice(1, 4);
        public static readonly Dice d6 = new Dice(1, 6);
        public static readonly Dice d8 = new Dice(1, 8);
        public static readonly Dice d10 = new Dice(1, 10);
        public static readonly Dice d12 = new Dice(1, 12);
        public static readonly Dice d20 = new Dice(1, 20);
        public static readonly Dice d100 = new Dice(1, 100);
        private static readonly List<Dice> Dices = new List<Dice>() { d1, d2, d3, d4, d6, d8, d10, d12, d20, d100 };
        private static List<DicesSet> DicesForSum = new List<DicesSet>();

        static DiceHelper()
        {
            DicesForSum.Add(new DicesSet()); // 0
            DicesForSum.Add(new DicesSet() { d1 }); // 0.5
            DicesForSum.Add(new DicesSet() { d1 }); // 1
            DicesForSum.Add(new DicesSet() { d2 }); // 1.5
            DicesForSum.Add(new DicesSet() { d3 }); // 2
            DicesForSum.Add(new DicesSet() { d4 }); // 2.5
            DicesForSum.Add(new DicesSet() { d4 }); // 3
            DicesForSum.Add(new DicesSet() { d6 }); // 3.5
            DicesForSum.Add(new DicesSet() { d6 }); // 4
            DicesForSum.Add(new DicesSet() { d8 }); // 4.5
            DicesForSum.Add(new DicesSet() { { d4, 2 } }); // 5
            DicesForSum.Add(new DicesSet() { d10 }); // 5.5
            DicesForSum.Add(new DicesSet() { d4, d6 }); // 6
            DicesForSum.Add(new DicesSet() { d12 }); // 6.5
            DicesForSum.Add(new DicesSet() { { d6, 2 } }); // 7
            DicesForSum.Add(new DicesSet() { { d6, 2 } }); // 7.5
            DicesForSum.Add(new DicesSet() { d6, d8 }); // 8
            DicesForSum.Add(new DicesSet() { d6, d8 }); // 8.5
            DicesForSum.Add(new DicesSet() { { d8, 2 } }); // 9
            DicesForSum.Add(new DicesSet() { { d8, 2 } }); // 9.5
            DicesForSum.Add(new DicesSet() { d8, d10 }); // 10
            DicesForSum.Add(new DicesSet() { d20 }); // 10.5
            DicesForSum.Add(new DicesSet() { { d10, 2 } }); // 11
            DicesForSum.Add(new DicesSet() { { d10, 2 } }); // 11.5
            DicesForSum.Add(new DicesSet() { d10, d12 }); // 12
            DicesForSum.Add(new DicesSet() { d10, d12 }); // 12.5
            DicesForSum.Add(new DicesSet() { { d12, 2 } }); // 13
            DicesForSum.Add(new DicesSet() { { d12, 2 } }); // 13.5
            DicesForSum.Add(new DicesSet() { d6, d20 }); // 14
            DicesForSum.Add(new DicesSet() { { d8, 2 }, d10 }); // 14.5
            DicesForSum.Add(new DicesSet() { d8, d20 }); // 15
            DicesForSum.Add(new DicesSet() { d8, { d10, 2 } }); // 15.5
            DicesForSum.Add(new DicesSet() { d10, d20 }); // 16
            DicesForSum.Add(new DicesSet() { { d10, 3 } }); // 16.5
            DicesForSum.Add(new DicesSet() { d12, d20 }); // 17
            DicesForSum.Add(new DicesSet() { { d10, 2 }, d12 }); // 17.5
            DicesForSum.Add(new DicesSet() { { d8, 4 } }); // 18
            DicesForSum.Add(new DicesSet() { d10, { d12, 2 } }); // 18.5
            DicesForSum.Add(new DicesSet() { { d8, 3 }, d10 }); // 19
            DicesForSum.Add(new DicesSet() { { d12, 3 } }); // 19.5
            DicesForSum.Add(new DicesSet() { { d8, 2 }, { d10, 2 } }); // 20
            DicesForSum.Add(new DicesSet() { { d8, 2 }, { d10, 2 } }); // 20.5
            DicesForSum.Add(new DicesSet() { { d20, 2 } }); // 21
        }

        public static string GetDices(double average)
        {
            if (average < 0)
            {
                throw new ArgumentException("The average cannot be negative!");
            }
            int sum = Convert.ToInt32(2 * average);
            if (sum < DicesForSum.Count)
            {
                return DicesForSum[sum].ToString();
            }
            else
            if (sum == DicesForSum.Count)
            {
                return DicesForSum[DicesForSum.Count - 1].ToString();
            }
            else
            {
                return DicesForSum[DicesForSum.Count - 1].ToString() + "+" + (sum + 1 - DicesForSum.Count) / 2;
            }
        }

        public static bool HasDice(string dice)
        {
            return Regex.IsMatch(dice, @"\d*[kd]\d+");
        }

        public static bool IsDice(string dice)
        {
            return Regex.IsMatch(dice, @"^\d*[kd]\d+$");
        }

        public static bool IsDiceOrNumber(string dice)
        {
            double val;
            return Regex.IsMatch(dice, @"^\d*[kd]\d+$") || double.TryParse(dice, out val);
        }

        public static double GetAverage(string dices)
        {
            int idx = dices.ToLower().IndexOf(" plus ");
            if (idx >= 0)
            {
                return GetAverageDices(dices.Substring(0, idx)) + GetAverageDices(dices.Substring(idx + 6));
            }
            return GetAverageDices(dices);
        }

        private static double GetAverageDices(string dices)
        {
            double result = 0;
            int idx = dices.IndexOf(" ");
            if (idx >= 0)
                dices = dices.Substring(0, idx);
            idx = dices.IndexOf("/");
            if (idx >= 0)
                dices = dices.Substring(0, idx);
            idx = dices.IndexOf("*");
            if (idx >= 0)
                dices = dices.Substring(0, idx);
            dices = dices.Replace("d", "k");
            bool minus = dices.Contains("-");
            string[] dicesStrings = Regex.Split(dices, @"[+-]");
            for (int i = 0; i < dicesStrings.Length; ++i)
            {
                if (IsDiceOrNumber(dicesStrings[i]))
                {
                    int number;
                    int dIdx = dicesStrings[i].IndexOf("k");
                    string numberString;
                    string diceName;
                    if (dIdx >= 0)
                    {
                        numberString = dicesStrings[i].Substring(0, dIdx);
                        diceName = dicesStrings[i].Substring(dIdx);
                    }
                    else
                    {
                        numberString = dicesStrings[i];
                        diceName = "1";
                    }
                    int.TryParse(numberString, out number);
                    if (minus && i == dicesStrings.Length - 1)
                        result -= number * Dices.FirstOrDefault(x => x.Name == diceName).Average;
                    else
                        result += number * Dices.FirstOrDefault(x => x.Name == diceName).Average;
                }
            }
            return result;
        }
    }

    class DicesSet : Dictionary<Dice, int>
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
            StringBuilder sb = new StringBuilder();
            foreach (var p in this.OrderByDescending(x => x.Key))
            {
                if (p.Value > 0)
                {
                    if (p.Value > 1)
                        sb.Append(p.Value);
                    sb.Append(p.Key);
                    sb.Append("+");
                }
            }
            if (sb.Length == 0) return "0";
            return sb.ToString(0, sb.Length - 1);
        }
    }

    class Dice : IComparable<Dice>
    {
        public string Name { get; }
        public int MinValue { get; }
        public int MaxValue { get; }
        public int Sum { get; }
        public double Average { get; }

        public Dice(int minValue, int maxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            Sum = MinValue + MaxValue;
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

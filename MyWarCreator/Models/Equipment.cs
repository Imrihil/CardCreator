using MyWarCreator.Helpers;
using MyWarCreator.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace MyWarCreator.Models
{
    class Equipment : AttackAbilityCard
    {
        public override string DescriptionFull
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (!string.IsNullOrEmpty(Description))
                    sb.AppendFormat("{0}\n\n", Description);
                if (Defence > 0)
                    sb.AppendFormat("Obrona: {0}\n\n", Defence.ToString());
                sb.Append(AttackDescription());
                return sb.ToString();
            }
        }

        public Equipment(IList<string> row, string dirPath) : base(dirPath)
        {
            int value;
            Type = row[0];
            Name = row[1];
            if (!string.IsNullOrEmpty(row[2]) ||
                !string.IsNullOrEmpty(row[3]) ||
                !string.IsNullOrEmpty(row[4]) ||
                !string.IsNullOrEmpty(row[5]) ||
                !string.IsNullOrEmpty(row[6]))
            {
                if (!string.IsNullOrEmpty(row[2])) LeftEffects.Add(row[2] + "S"); else LeftEffects.Add("");
                if (!string.IsNullOrEmpty(row[3])) LeftEffects.Add(row[3] + "Z"); else LeftEffects.Add("");
                if (!string.IsNullOrEmpty(row[4])) LeftEffects.Add(row[4] + "W"); else LeftEffects.Add("");
                if (!string.IsNullOrEmpty(row[5])) LeftEffects.Add(row[5] + "M"); else LeftEffects.Add("");
                if (!string.IsNullOrEmpty(row[6])) LeftEffects.Add(row[6] + "U"); else LeftEffects.Add("");
            }
            if (!string.IsNullOrEmpty(row[7]) ||
                !string.IsNullOrEmpty(row[8]) ||
                !string.IsNullOrEmpty(row[9]) ||
                !string.IsNullOrEmpty(row[10]) ||
                !string.IsNullOrEmpty(row[11]))
            {
                if (!string.IsNullOrEmpty(row[7])) RightEffects.Add((row[7] == "1" ? (row[29] == "TAK" ? "1" : "") : row[7]) + (row[29] == "TAK" ? "" : "k4")); else RightEffects.Add("");
                if (!string.IsNullOrEmpty(row[8])) RightEffects.Add((row[8] == "1" ? (row[29] == "TAK" ? "1" : "") : row[8]) + (row[29] == "TAK" ? "" : "k6")); else RightEffects.Add("");
                if (!string.IsNullOrEmpty(row[9])) RightEffects.Add((row[9] == "1" ? (row[29] == "TAK" ? "1" : "") : row[9]) + (row[29] == "TAK" ? "" : "k8")); else RightEffects.Add("");
                if (!string.IsNullOrEmpty(row[10])) RightEffects.Add((row[10] == "1" ? (row[29] == "TAK" ? "1" : "") : row[10]) + (row[29] == "TAK" ? "" : "k10")); else RightEffects.Add("");
                if (!string.IsNullOrEmpty(row[11])) RightEffects.Add((row[11] == "1" ? (row[29] == "TAK" ? "1" : "") : row[11]) + (row[29] == "TAK" ? "" : "k12")); else RightEffects.Add("");
            }
            ProcessRow(row.Skip(12).ToList());
            int.TryParse(row[29], out value);
            Defence = value;
            Description = row[30];
            int.TryParse(row[31], out value);
            Price = value;

            CalculateTypeArea();

            MainImage = LoadImage(dirPath, Name);
            LeftEffectsImage = LoadImage(cardsDirPath, "left-stats");
        }
    }
}

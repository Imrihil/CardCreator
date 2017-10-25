using MyWarCreator.Extensions;
using MyWarCreator.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace MyWarCreator.Models
{
    class Armour : Equipment
    {
        public int ArmourValue { get; set; }
        public override string DescriptionFull
        {
            get
            {
                return Description;
            }
        }
        public Armour(IList<string> row, string dirPath) : base(row, dirPath)
        {
            if (row.Count < 13)
                throw new ArgumentException("W wierszu znajduje się za mało kolumn by utworzyć kartę pancerza!");
            int value;
            try
            {
                int.TryParse(row[12], out value);
                ArmourValue = value;
            }
            catch (Exception)
            {
                throw new ArgumentException("Kolumna 12 powinna zawierać liczbę!");
            }
            if (File.Exists(dicesDirPath + "/default.png"))
                DiceImage = Image.FromFile(dicesDirPath + "/default.png");
        }
        public override void DrawCard(Graphics graphics)
        {
            base.DrawCard(graphics);
            using (Font font = new Font(FontsHelper.pfc.Families.FirstOrDefault(x => x.Name.Contains("Trebuchet MS")), 18, FontStyle.Bold, GraphicsUnit.Pixel))
                graphics.DrawAdjustedString(ArmourValue.ToString(), font, Brushes.Black, DiceTextArea, FontsHelper.StringFormatCentered, 6);
        }
    }
}

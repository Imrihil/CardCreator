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
        public override string DescriptionFull
        {
            get
            {
                return Description;
            }
        }
        public Armour(IList<string> row, string dirPath) : base(row, dirPath)
        {
            int value;
            if (row[4] != "NULL") RunePlaces = "A\nB\nC\nD\nE";
            if (row.Count < 22 || !int.TryParse(row[3], out value))
                throw new ArgumentException("W wierszu znajduje się za mało kolumn by utworzyć kartę pancerza!");
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

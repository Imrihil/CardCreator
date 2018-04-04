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
    class Equipment : Card
    {
        public Image UpgradesImage { get; set; }
        public Rectangle UpgradesArea { get; set; } = new Rectangle(295, 110, 40, 160);
        public int Price { get; set; }
        public Rectangle PriceArea { get; set; } = new Rectangle(19, 431, 43, 43);
        public Image PriceImage { get; set; }
        public Rectangle PriceImageArea { get; set; } = new Rectangle(3, 409, 86, 86);
        public Equipment(IList<string> row, string dirPath) : base(dirPath)
        {
            TypeArea = new Rectangle(79, 452, 253, 18);
            if (row.Count < 5)
                throw new ArgumentException("W wierszu znajduje się za mało kolumn by utworzyć kartę!");
            Type = row[0];
            Name = row[1];
            Dmg = row[2];
            Rune = row[3];
            if (Rune == "NULL") Rune = null;
            Description = row[4];
            int value;
            int.TryParse(row[13], out value);
            Price = value;
            string backgroundPath = dirPath + "/background.png";
            if (File.Exists(backgroundPath))
                BackgroundImage = Image.FromFile(backgroundPath);
            string mainImagePath = dirPath + "/" + Name + ".png";
            if (!File.Exists(mainImagePath))
                mainImagePath = dirPath + "/" + Name + ".jpg";
            if (File.Exists(mainImagePath))
                MainImage = Image.FromFile(mainImagePath);
            if (File.Exists(dicesDirPath + "/price.png"))
                PriceImage = Image.FromFile(dicesDirPath + "/price.png");
        }
        public override void DrawCard(Graphics graphics)
        {
            base.DrawCard(graphics);
            if (UpgradesImage != null)
                DrawingHelper.MapDrawing(graphics, UpgradesImage, UpgradesArea);
            if (PriceImage != null)
                DrawingHelper.MapDrawing(graphics, PriceImage, PriceImageArea);
            if (Price > 0)
            {
                using (Font font = new Font(FontsHelper.pfc.Families.FirstOrDefault(x => x.Name.Contains("Trebuchet MS")), 24, FontStyle.Regular, GraphicsUnit.Pixel))
                    graphics.DrawAdjustedString(DescriptionHelper.ToRoman(Price), font, Brushes.Black, PriceArea, FontsHelper.StringFormatCentered);
            }
        }
    }
}

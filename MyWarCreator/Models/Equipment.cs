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
#if DEBUG
        protected const string dicesDirPath = @"../../dices";
#else
        protected const string dicesDirPath = @"./dices";
#endif
        public Image MainImage { get; set; }
        public Rectangle MainImageArea { get; set; } = new Rectangle(65, 40, 230, 230);
        public Image DiceImage { get; set; }
        public Rectangle DiceArea { get; set; } = new Rectangle(235, 20, 110, 110);
        public Rectangle DiceTextArea { get; set; } = new Rectangle(245, 30, 90, 90);
        public Image UpgradesImage { get; set; }
        public Rectangle UpgradesArea { get; set; } = new Rectangle(295, 120, 40, 160);
        public Equipment(IList<string> row, string dirPath) : base(row, dirPath)
        {
            if (row.Count < 5)
                throw new ArgumentException("W wierszu znajduje się za mało kolumn by utworzyć kartę!");
            Type = row[0];
            Name = row[1];
            Dmg = row[2];
            Rune = row[3];
            if (Rune == "NULL") Rune = null;
            Description = row[4];
            string backgroundPath = dirPath + "/background.png";
            if (File.Exists(backgroundPath))
                BackgroundImage = Image.FromFile(backgroundPath);
            string mainImagePath = dirPath + "/" + Name + ".png";
            if (File.Exists(mainImagePath))
                MainImage = Image.FromFile(mainImagePath);
        }
        public override void DrawCard(Graphics graphics)
        {
            base.DrawCard(graphics);
            if (MainImage != null)
                DrawingHelper.MapDrawing(graphics, MainImage, MainImageArea);
            if (DiceImage != null)
            {
                DrawingHelper.MapDrawing(graphics, DiceImage, DiceArea);
            }
            if (UpgradesImage != null)
            {
                DrawingHelper.MapDrawing(graphics, UpgradesImage, UpgradesArea);
            }
        }
    }
}

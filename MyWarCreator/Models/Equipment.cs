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
        public Equipment(IList<string> row, string dirPath) : base(dirPath)
        {
            TypeArea = new Rectangle(79, 452, 253, 18);
            if (row.Count < 22)
                throw new ArgumentException("W wierszu znajduje się za mało kolumn by utworzyć kartę!");
            int value;
            Type = row[0];
            Name = row[1];
            Description = row[5];
            int.TryParse(row[21], out value);
            Price = value;
            string backgroundPath = dirPath + "/background.png";
            if (File.Exists(backgroundPath))
                BackgroundImage = Image.FromFile(backgroundPath);
            string mainImagePath = dirPath + "/" + Name + ".png";
            if (!File.Exists(mainImagePath))
                mainImagePath = dirPath + "/" + Name + ".jpg";
            if (File.Exists(mainImagePath))
                MainImage = Image.FromFile(mainImagePath);
        }

        public override void DrawCard(Graphics graphics)
        {
            base.DrawCard(graphics);
        }
    }
}

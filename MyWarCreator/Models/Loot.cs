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
    class Loot : Equipment
    {
        public Loot(IList<string> row, string dirPath) : base(row, dirPath)
        {
            InitLoot(dirPath, "");
        }
        public Loot(IList<string> row, string dirPath, string id) : base(row, dirPath)
        {
            InitLoot(dirPath, id);
        }

        private void InitLoot(string dirPath, string id)
        {
            if (DescriptionFull.Length < 3)
            {
                DescriptionFont = 64;
            }

            string backgroundPath = dirPath + "/background_loot.png";
            if (File.Exists(backgroundPath))
                BackgroundImage = Image.FromFile(backgroundPath);
            string framePath = dirPath + "/frame.png";
            if (File.Exists(framePath))
                MainImageFrame = Image.FromFile(framePath);
            string mainImagePath = dirPath + "/" + Name + " " + Rune + id + ".png";
            if (!File.Exists(mainImagePath))
                mainImagePath = dirPath + "/" + Name + " " + Rune + id + ".jpg";
            if (File.Exists(mainImagePath))
                MainImage = Image.FromFile(mainImagePath);
        }
    }
}

using MyWarCreator.Extensions;
using MyWarCreator.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace MyWarCreator.Models
{
    class Card
    {
        public virtual string Type { get; set; }
        public string Name { get; set; }
        public string Dmg { get; set; }
        public string Rune { get; set; }
        public string RunePlaces { get; set; }
        public Rectangle RunesArea { get; set; } = new Rectangle(25, 35, 45, 250);
        public string Description { get; set; }
        public string resultsDirPath { get; set; }
        public Image BackgroundImage { get; set; }
        public Rectangle NameArea { get; set; } = new Rectangle(65, 285, 230, 32);
        public Rectangle DescriptionArea { get; set; } = new Rectangle(35, 335, 290, 115);
        public Rectangle TypeArea { get; set; } = new Rectangle(35, 455, 290, 20);
        public virtual string DescriptionFull
        {
            get
            {
                return Description;
            }
        }

        public Card(IList<string> row, string dirPath)
        {
            resultsDirPath = dirPath + "/results";
        }
        public virtual void DrawCard(Graphics graphics)
        {
            if (Name != null)
            {
                using (Font font = new Font(FontsHelper.pfc.Families.FirstOrDefault(x => x.Name.Contains("Akvaleir")), 18, FontStyle.Bold, GraphicsUnit.Pixel))
                    graphics.DrawAdjustedString(Name.ToUpper(), font, Brushes.Black, NameArea, FontsHelper.StringFormatCentered, 6);
            }
            if (DescriptionFull != null)
            {
                using (Font font = new Font(FontsHelper.pfc.Families.FirstOrDefault(x => x.Name.Contains("Trebuchet MS")), 10, FontStyle.Regular, GraphicsUnit.Pixel))
                    graphics.DrawAdjustedString(DescriptionFull, font, Brushes.Black, DescriptionArea, FontsHelper.StringFormatCentered, 6);
            }
            if (Type != null)
            {
                using (Font font = new Font(FontsHelper.pfc.Families.FirstOrDefault(x => x.Name.Contains("Akvaleir")), 12, FontStyle.Bold, GraphicsUnit.Pixel))
                    graphics.DrawAdjustedString(Type.ToUpper(), font, Brushes.Black, TypeArea, FontsHelper.StringFormatCentered, 6);
            }
            if (RunePlaces != null)
            {
                using (Font font = new Font(FontsHelper.pfc.Families.FirstOrDefault(x => x.Name.Contains("Runic AltNo")), 64, FontStyle.Bold, GraphicsUnit.Pixel))
                    graphics.DrawString(RunePlaces.ToUpper(), font, new SolidBrush(Color.FromArgb(196, Color.White)), RunesArea);
            }
            if (Rune != null)
            {
                using (Font font = new Font(FontsHelper.pfc.Families.FirstOrDefault(x => x.Name.Contains("Runic AltNo")), 64, FontStyle.Bold, GraphicsUnit.Pixel))
                    graphics.DrawString(Rune.ToUpper(), font, Brushes.Black, RunesArea);
            }
        }
        public string GenerateFile(string fileNamePrefix = "", string fileNameSuffix = "")
        {
            try
            {
                if (!Directory.Exists(resultsDirPath))
                {
                    Directory.CreateDirectory(resultsDirPath);
                }
                string savePath = Path.GetFullPath(resultsDirPath + "/" + fileNamePrefix + Name + fileNameSuffix + ".png");
                using (Image bitmap = new Bitmap(BackgroundImage))
                {
                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        DrawCard(graphics);
                        bitmap.Save(savePath, ImageFormat.Png);
                    }
                }
                return $"Pomyślnie wygenerowano kartę {Name}.";
            }
            catch (Exception ex)
            {
                return $"Podczas generowania karty {Name} wystąpił błąd: " + ex.Message;
            }
        }
    }
}

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
#if DEBUG
        protected const string dicesDirPath = @"../../dices";
#else
        protected const string dicesDirPath = @"./dices";
#endif
        public virtual string Type { get; set; }
        public Rectangle TypeArea { get; set; } = new Rectangle(29, 452, 303, 18);
        public string Name { get; set; }
        public Rectangle NameArea { get; set; } = new Rectangle(72, 268, 217, 32);
        public string Dmg { get; set; }
        public Image DiceImage { get; set; }
        public Rectangle DiceArea { get; set; } = new Rectangle(255, 0, 110, 110);
        public Rectangle DiceTextArea { get; set; } = new Rectangle(265, 10, 90, 90);
        public string Rune { get; set; }
        public string RunePlaces { get; set; }
        public Rectangle RunesArea { get; set; } = new Rectangle(29, 35, 43, 232);
        public string Description { get; set; }
        public Rectangle DescriptionArea { get; set; } = new Rectangle(29, 317, 303, 133);
        public float DescriptionFont { get; set; } = 10;
        public Image MainImage { get; set; }
        public Image MainImageFrame { get; set; }
        public Rectangle MainImageArea { get; set; } = new Rectangle(65, 35, 230, 230);
        public Image BackgroundImage { get; set; }
        public string ResultsDirPath { get; set; }
        public virtual string FileName { get { return $"{Type} - {Name}"; } }
        public virtual string DescriptionFull
        {
            get
            {
                return Description;
            }
        }

        public Card(string dirPath)
        {
            ResultsDirPath = dirPath + "/results";
        }

        public virtual void DrawCard(Graphics graphics)
        {
            if (MainImage != null)
                DrawingHelper.MapDrawing(graphics, MainImage, MainImageArea);
            if (MainImageFrame != null)
                DrawingHelper.MapDrawing(graphics, MainImageFrame, MainImageArea);
            if (DiceImage != null)
                DrawingHelper.MapDrawing(graphics, DiceImage, DiceArea);
            if (Name != null)
            {
                using (Font font = new Font(FontsHelper.pfc.Families.FirstOrDefault(x => x.Name.Contains("Akvaleir")), 18, FontStyle.Bold, GraphicsUnit.Pixel))
                    graphics.DrawAdjustedString(Name.ToUpper(), font, Brushes.Black, NameArea, FontsHelper.StringFormatCentered, 6);
            }
            if (DescriptionFull != null)
            {
                using (Font font = new Font(FontsHelper.pfc.Families.FirstOrDefault(x => x.Name.Contains("Trebuchet MS")), DescriptionFont, FontStyle.Regular, GraphicsUnit.Pixel))
                    graphics.DrawAdjustedString(DescriptionFull, font, Brushes.Black, DescriptionArea, FontsHelper.StringFormatCentered);
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
            if (Dmg != null)
            {
                using (Font font = new Font(FontsHelper.pfc.Families.FirstOrDefault(x => x.Name.Contains("Trebuchet MS")), 18, FontStyle.Bold, GraphicsUnit.Pixel))
                    graphics.DrawAdjustedString(Dmg, font, Brushes.Black, DiceTextArea, FontsHelper.StringFormatCentered, 6, 18, true, false);
            }
        }
        public string GenerateFile(string fileNamePrefix = "", string fileNameSuffix = "")
        {
            try
            {
                if (!Directory.Exists(ResultsDirPath))
                {
                    Directory.CreateDirectory(ResultsDirPath);
                }
                string savePath = Path.GetFullPath(ResultsDirPath + "/" + fileNamePrefix + FileName + fileNameSuffix + ".png");
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
        public void LoadDiceImage()
        {
            if ((Dmg.ToLower().Contains("d20") || Dmg.ToLower().Contains("k20")) && File.Exists(dicesDirPath + "/d20.png"))
                DiceImage = Image.FromFile(dicesDirPath + "/d20.png");
            else if ((Dmg.ToLower().Contains("d12") || Dmg.ToLower().Contains("k12")) && File.Exists(dicesDirPath + "/d12.png"))
                DiceImage = Image.FromFile(dicesDirPath + "/d12.png");
            else if ((Dmg.ToLower().Contains("d10") || Dmg.ToLower().Contains("k10")) && File.Exists(dicesDirPath + "/d10.png"))
                DiceImage = Image.FromFile(dicesDirPath + "/d10.png");
            else if ((Dmg.ToLower().Contains("d8") || Dmg.ToLower().Contains("k8")) && File.Exists(dicesDirPath + "/d8.png"))
                DiceImage = Image.FromFile(dicesDirPath + "/d8.png");
            else if ((Dmg.ToLower().Contains("d6") || Dmg.ToLower().Contains("k6")) && File.Exists(dicesDirPath + "/d6.png"))
                DiceImage = Image.FromFile(dicesDirPath + "/d6.png");
            else if ((Dmg.ToLower().Contains("d4") || Dmg.ToLower().Contains("k4")) && File.Exists(dicesDirPath + "/d4.png"))
                DiceImage = Image.FromFile(dicesDirPath + "/d4.png");
            else if (File.Exists(dicesDirPath + "/default.png"))
                DiceImage = Image.FromFile(dicesDirPath + "/default.png");
        }
    }
}

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
        protected const string cardsDirPath = @"../../AppData/cards";
#else
        protected const string cardsDirPath = @"./cards";
#endif
        public virtual string Type { get; set; }
        public Rectangle TypeArea { get; set; } = new Rectangle(130, 460, 205, 15);
        public string Name { get; set; }
        public Rectangle NameArea { get; set; } = new Rectangle(70, 245, 220, 40);
        public List<string> LeftEffects { get; set; } = new List<string>();
        public Image LeftEffectsImage { get; set; }
        public Rectangle LeftEffectsImageArea { get; set; } = new Rectangle(0, 0, 85, 280);
        public List<Rectangle> LeftEffectsArea { get; set; } = new List<Rectangle>() { new Rectangle(10, 20, 60, 40), new Rectangle(10, 70, 60, 40), new Rectangle(10, 120, 60, 40), new Rectangle(10, 170, 60, 40), new Rectangle(10, 220, 60, 40) };
        public List<string> RightEffects { get; set; } = new List<string>();
        public Image RightEffectsImage { get; set; }
        public Rectangle RightEffectsImageArea { get; set; } = new Rectangle(275, 0, 85, 280);
        public List<Rectangle> RightEffectsArea { get; set; } = new List<Rectangle>() { new Rectangle(290, 20, 60, 40), new Rectangle(290, 70, 60, 40), new Rectangle(290, 120, 60, 40), new Rectangle(290, 170, 60, 40), new Rectangle(290, 220, 60, 40) };
        public int Price { get; set; }
        public Image PriceImage { get; set; }
        public Rectangle PriceImageArea { get; set; } = new Rectangle(20, 440, 40, 40);
        public string Description { get; set; }
        public virtual string DescriptionFull { get { return Description; } }
        public Rectangle DescriptionArea { get; set; } = new Rectangle(30, 285, 300, 170);
        public Image MainImage { get; set; }
        public Rectangle MainImageArea { get; set; } = new Rectangle(70, 25, 220, 220);
        public Image FrontImage { get; set; }
        public Rectangle FrontImageArea { get; set; } = new Rectangle(0, 0, 360, 500);
        public Image BackgroundImage { get; set; }
        public string ResultsDirPath { get; set; }
        public virtual string FileName { get { return $"{Type} - {Name}"; } }

        public Card(string dirPath)
        {
            ResultsDirPath = dirPath + "/results";
            string leftEffectsImagePath = cardsDirPath + "/left.png";
            if (File.Exists(leftEffectsImagePath))
                LeftEffectsImage = Image.FromFile(leftEffectsImagePath);
            string rightEffectsImagePathImagePath = cardsDirPath + "/right.png";
            if (File.Exists(rightEffectsImagePathImagePath))
                RightEffectsImage = Image.FromFile(rightEffectsImagePathImagePath);
            string priceImagePath = cardsDirPath + "/price.png";
            if (File.Exists(priceImagePath))
                PriceImage = Image.FromFile(priceImagePath);
            string frontImagePath = cardsDirPath + "/front.png";
            if (File.Exists(frontImagePath))
                FrontImage = Image.FromFile(frontImagePath);
            string backgroundImagePath = cardsDirPath + "/background.png";
            if (File.Exists(backgroundImagePath))
                BackgroundImage = Image.FromFile(backgroundImagePath);
        }

        public virtual void DrawCard(Graphics graphics)
        {
            if (MainImage != null)
                DrawingHelper.MapDrawing(graphics, MainImage, MainImageArea);
            if (FrontImage != null)
                DrawingHelper.MapDrawing(graphics, FrontImage, FrontImageArea);
            if (LeftEffects.Any() && LeftEffectsImage != null)
                DrawingHelper.MapDrawing(graphics, LeftEffectsImage, LeftEffectsImageArea);
            if (RightEffects.Any() && RightEffectsImage != null)
                DrawingHelper.MapDrawing(graphics, RightEffectsImage, RightEffectsImageArea);
            if (Name != null)
            {
                using (Font font = new Font(FontsHelper.pfc.Families.FirstOrDefault(x => x.Name.Contains("Akvaleir")), 24, FontStyle.Bold, GraphicsUnit.Pixel))
                    graphics.DrawAdjustedString(Name.ToUpper(), font, Brushes.White, NameArea, FontsHelper.StringFormatCentered, 6);
            }
            if (DescriptionFull != null)
            {
                using (Font font = new Font(FontsHelper.pfc.Families.FirstOrDefault(x => x.Name.Contains("Trebuchet MS")), 12, FontStyle.Regular, GraphicsUnit.Pixel))
                    graphics.DrawAdjustedString(DescriptionFull, font, Brushes.White, DescriptionArea, FontsHelper.StringFormatCentered);
            }
            if (Type != null)
            {
                using (Font font = new Font(FontsHelper.pfc.Families.FirstOrDefault(x => x.Name.Contains("Trebuchet MS")), 12, FontStyle.Regular, GraphicsUnit.Pixel))
                    graphics.DrawAdjustedString(Type.ToUpper(), font, Brushes.White, TypeArea, FontsHelper.StringFormatCentered, 6);
            }
            float leftEffectsFont = 20;
            for(int i = 0; i < LeftEffects.Count; ++i)
            {
                using (Font font = new Font(FontsHelper.pfc.Families.FirstOrDefault(x => x.Name.Contains("Trebuchet MS")), leftEffectsFont, FontStyle.Bold, GraphicsUnit.Pixel))
                    leftEffectsFont = FontsHelper.GetAdjustedFont(graphics, LeftEffects[i], font, LeftEffectsArea[i], FontsHelper.StringFormatCentered, 6, (int)leftEffectsFont, true, false).Size;
            }
            for (int i = 0; i < LeftEffects.Count; ++i)
            {
                using (Font font = new Font(FontsHelper.pfc.Families.FirstOrDefault(x => x.Name.Contains("Trebuchet MS")), leftEffectsFont, FontStyle.Bold, GraphicsUnit.Pixel))
                    graphics.DrawAdjustedString(LeftEffects[i], font, Brushes.White, LeftEffectsArea[i], FontsHelper.StringFormatCentered, 6, (int)leftEffectsFont, true, false);
            }
            float rightEffectsFont = 20;
            for (int i = 0; i < RightEffects.Count; ++i)
            {
                using (Font font = new Font(FontsHelper.pfc.Families.FirstOrDefault(x => x.Name.Contains("Trebuchet MS")), rightEffectsFont, FontStyle.Bold, GraphicsUnit.Pixel))
                    rightEffectsFont = FontsHelper.GetAdjustedFont(graphics, RightEffects[i], font, RightEffectsArea[i], FontsHelper.StringFormatCentered, 6, (int)rightEffectsFont, true, false).Size;
            }
            for (int i = 0; i < RightEffects.Count; ++i)
            {
                using (Font font = new Font(FontsHelper.pfc.Families.FirstOrDefault(x => x.Name.Contains("Trebuchet MS")), rightEffectsFont, FontStyle.Bold, GraphicsUnit.Pixel))
                    graphics.DrawAdjustedString(RightEffects[i], font, Brushes.White, RightEffectsArea[i], FontsHelper.StringFormatCentered, 6, (int)rightEffectsFont, true, false);
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
    }
}

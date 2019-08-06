using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using MyWarCreator.Extensions;
using MyWarCreator.Helpers;

namespace MyWarCreator.Models
{
    public class Card
    {
#if DEBUG
        protected const string CardsDirPath = @"../../AppData/cards";
#else
        protected const string CardsDirPath = @"./cards";
#endif
        protected string Type { get; set; }
        protected Rectangle TypeArea { get; set; } = new Rectangle(25, 460, 310, 15);
        public string Name { get; protected set; }
        private Rectangle NameArea { get; } = new Rectangle(70, 245, 220, 40);
        protected List<string> LeftEffects { get; } = new List<string>();
        protected Image LeftEffectsImage { get; set; }
        private Rectangle LeftEffectsImageArea { get; } = new Rectangle(0, 0, 85, 280);
        private List<Rectangle> LeftEffectsArea { get; } = new List<Rectangle> { new Rectangle(10, 20, 60, 40), new Rectangle(10, 70, 60, 40), new Rectangle(10, 120, 60, 40), new Rectangle(10, 170, 60, 40), new Rectangle(10, 220, 60, 40) };
        protected List<string> RightEffects { get; } = new List<string>();
        protected Image RightEffectsImage { private get; set; }
        private Rectangle RightEffectsImageArea { get; } = new Rectangle(275, 0, 85, 280);
        private List<Rectangle> RightEffectsArea { get; } = new List<Rectangle> { new Rectangle(290, 20, 60, 40), new Rectangle(290, 70, 60, 40), new Rectangle(290, 120, 60, 40), new Rectangle(290, 170, 60, 40), new Rectangle(290, 220, 60, 40) };
        protected int Price { get; set; }
        private Image PriceImage { get; }
        private Rectangle PriceImageArea { get; } = new Rectangle(20, 440, 40, 40);
        protected string Description { get; set; }
        protected virtual string DescriptionFull => Description;
        private Rectangle DescriptionArea { get; } = new Rectangle(30, 285, 300, 170);
        protected Image MainImage { get; set; }
        private Rectangle MainImageArea { get; } = new Rectangle(70, 25, 220, 220);
        private Image FrontImage { get; }
        private Rectangle FrontImageArea { get; } = new Rectangle(0, 0, 360, 500);
        private Image BackgroundImage { get; }
        private string ResultsDirPath { get; }
        protected virtual string FileName => $"{Type} - {Name}";
        protected int PriceLimit { get; } = 3;

        protected Card(string dirPath)
        {
            ResultsDirPath = dirPath + "/results";
            LeftEffectsImage = LoadImage(CardsDirPath, "left");
            RightEffectsImage = LoadImage(CardsDirPath, "right");
            PriceImage = LoadImage(CardsDirPath, "price");
            FrontImage = LoadImage(CardsDirPath, "front");
            BackgroundImage = LoadImage(CardsDirPath, "background");
        }

        protected virtual void DrawCard(Graphics graphics)
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
                using (var font = new Font(FontsHelper.Pfc.Families.FirstOrDefault(x => x.Name.Contains("Akvaleir")) ?? FontFamily.GenericSansSerif,
                    24, FontStyle.Bold, GraphicsUnit.Pixel))
                    graphics.DrawAdjustedString(Name.ToUpper(), font, Brushes.White, NameArea, FontsHelper.StringFormatCentered, 6);
            }
            if (DescriptionFull != null)
            {
                using (var font = new Font(FontsHelper.Pfc.Families.FirstOrDefault(x => x.Name.Contains("Trebuchet MS")) ?? FontFamily.GenericSansSerif,
                    12, FontStyle.Regular, GraphicsUnit.Pixel))
                    graphics.DrawAdjustedString(DescriptionFull, font, Brushes.White, DescriptionArea, FontsHelper.StringFormatCentered);
            }
            if (Type != null)
            {
                using (var font = new Font(FontsHelper.Pfc.Families.FirstOrDefault(x => x.Name.Contains("Trebuchet MS")) ?? FontFamily.GenericSansSerif,
                    12, FontStyle.Regular, GraphicsUnit.Pixel))
                    graphics.DrawAdjustedString(Type.ToUpper(), font, Brushes.White, TypeArea, FontsHelper.StringFormatCentered, 6);
            }
            float leftEffectsFont = 20;
            for (var i = 0; i < LeftEffects.Count && i < LeftEffectsArea.Count; ++i)
            {
                if (string.IsNullOrEmpty(LeftEffects[i])) continue;

                using (var font = new Font(FontsHelper.Pfc.Families.FirstOrDefault(x => x.Name.Contains("Trebuchet MS")) ?? FontFamily.GenericSansSerif,
                    leftEffectsFont, FontStyle.Bold, GraphicsUnit.Pixel))
                    leftEffectsFont = FontsHelper.GetAdjustedFont(graphics, LeftEffects[i], font, LeftEffectsArea[i], FontsHelper.StringFormatCentered, 6, (int)leftEffectsFont, true, false).Size;
            }
            for (var i = 0; i < LeftEffects.Count && i < LeftEffectsArea.Count; ++i)
            {
                using (var font = new Font(FontsHelper.Pfc.Families.FirstOrDefault(x => x.Name.Contains("Trebuchet MS")) ?? FontFamily.GenericSansSerif,
                    leftEffectsFont, FontStyle.Bold, GraphicsUnit.Pixel))
                    graphics.DrawAdjustedString(LeftEffects[i], font, Brushes.White, LeftEffectsArea[i], FontsHelper.StringFormatCentered, 6, (int)leftEffectsFont, true, false);
            }
            float rightEffectsFont = 20;
            for (var i = 0; i < RightEffects.Count && i < RightEffectsArea.Count; ++i)
            {
                if (string.IsNullOrEmpty(RightEffects[i])) continue;

                using (var font = new Font(FontsHelper.Pfc.Families.FirstOrDefault(x => x.Name.Contains("Trebuchet MS")) ?? FontFamily.GenericSansSerif,
                    rightEffectsFont, FontStyle.Bold, GraphicsUnit.Pixel))
                    rightEffectsFont = FontsHelper.GetAdjustedFont(graphics, RightEffects[i], font, RightEffectsArea[i], FontsHelper.StringFormatCentered, 6, (int)rightEffectsFont, true, false).Size;
            }
            for (var i = 0; i < RightEffects.Count && i < RightEffectsArea.Count; ++i)
            {
                using (var font = new Font(FontsHelper.Pfc.Families.FirstOrDefault(x => x.Name.Contains("Trebuchet MS")) ?? FontFamily.GenericSansSerif,
                    rightEffectsFont, FontStyle.Bold, GraphicsUnit.Pixel))
                    graphics.DrawAdjustedString(RightEffects[i], font, Brushes.White, RightEffectsArea[i], FontsHelper.StringFormatCentered, 6, (int)rightEffectsFont, true, false);
            }
            if (PriceImage != null)
            {
                if (Price <= 0) return;

                if (Price <= PriceLimit)
                {
                    for (var i = 0; i < Price; ++i)
                    {
                        var priceImageAreaI = new Rectangle(PriceImageArea.X + i * PriceImageArea.Width, PriceImageArea.Y, PriceImageArea.Width, PriceImageArea.Height);
                        DrawingHelper.MapDrawing(graphics, PriceImage, priceImageAreaI);
                    }
                }
                else
                {
                    var priceImageAreaI = new Rectangle(PriceImageArea.X + 5, PriceImageArea.Y, PriceImageArea.Width - 5, PriceImageArea.Height);
                    using (var font = new Font(FontsHelper.Pfc.Families.FirstOrDefault(x => x.Name.Contains("Trebuchet MS")), 12, FontStyle.Bold, GraphicsUnit.Pixel))
                        graphics.DrawAdjustedString(Price.ToString(), font, Brushes.White, priceImageAreaI, FontsHelper.StringFormatCentered, 6, 12, true, false);
                    priceImageAreaI = new Rectangle(PriceImageArea.X + PriceImageArea.Width, PriceImageArea.Y, PriceImageArea.Width, PriceImageArea.Height);
                    DrawingHelper.MapDrawing(graphics, PriceImage, priceImageAreaI);
                }
            }
            else
            {
                using (var font = new Font(FontsHelper.Pfc.Families.FirstOrDefault(x => x.Name.Contains("Trebuchet MS")), 12, FontStyle.Bold, GraphicsUnit.Pixel))
                    graphics.DrawAdjustedString(Price.ToString(), font, Brushes.White, PriceImageArea, FontsHelper.StringFormatCentered, 6, 12, true, false);
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
                var savePath = Path.GetFullPath(ResultsDirPath + "/" + fileNamePrefix + FileName + fileNameSuffix + ".png");
                using (Image bitmap = new Bitmap(BackgroundImage))
                {
                    using (var graphics = Graphics.FromImage(bitmap))
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

        protected Image LoadImage(string dirPath, string name)
        {
            var imagePath = dirPath + "/" + name + ".png";
            if (!File.Exists(imagePath))
                imagePath = dirPath + "/" + name + ".jpg";
            if (!File.Exists(imagePath))
                imagePath = dirPath + "/" + name.ToLower() + ".png";
            if (!File.Exists(imagePath))
                imagePath = dirPath + "/" + name.ToLower() + ".jpg";
            return File.Exists(imagePath)
                ? Image.FromFile(imagePath)
                : null;
        }

        protected virtual void CalculateTypeArea()
        {
            if (Price <= 0) return;

            TypeArea = Price <= PriceLimit
                ? new Rectangle(TypeArea.X + PriceImageArea.Width * Price, TypeArea.Y, TypeArea.Width - PriceImageArea.Width * Price, TypeArea.Height)
                : new Rectangle(TypeArea.X + PriceImageArea.Width * 2, TypeArea.Y, TypeArea.Width - PriceImageArea.Width * 2, TypeArea.Height);
        }
    }
}

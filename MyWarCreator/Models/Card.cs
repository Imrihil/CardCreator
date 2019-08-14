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
        private Rectangle LeftEffectsArea { get; } = new Rectangle(10, 20, 60, 40);
        private Point LeftEffectsAreaShift { get; } = new Point(0, 50);
        protected List<string> RightEffects { get; } = new List<string>();
        protected Image RightEffectsImage { get; set; }
        protected Rectangle RightEffectsImageArea { get; set; } = new Rectangle(275, 0, 85, 280);
        private Rectangle RightEffectsArea { get; } = new Rectangle(290, 20, 60, 40);
        protected Point RightEffectsAreaShift { get; } = new Point(0, 50);
        protected int Price { get; set; }
        private Image PriceImage { get; }
        private Rectangle PriceImageArea { get; } = new Rectangle(20, 440, 40, 40);
        protected string Description { get; set; }
        protected virtual string DescriptionFull => Description;
        protected Rectangle DescriptionArea { get; set; } = new Rectangle(30, 285, 300, 170);
        protected Image MainImage { get; set; }
        private Rectangle MainImageArea { get; } = new Rectangle(70, 25, 220, 220);
        private Image FrontImage { get; }
        private Rectangle FrontImageArea { get; } = new Rectangle(0, 0, 360, 500);
        private Image BackgroundImage { get; }
        private string ResultsDirPath { get; }
        protected virtual string FileName => $"{Type} - {Name}";
        protected int PriceLimit { get; } = 3;

        protected readonly FontFamily FontTrebuchetMs = FontsHelper.Pfc.Families.FirstOrDefault(x => x.Name.Contains("Trebuchet MS")) ?? FontFamily.GenericSansSerif;
        private readonly FontFamily fontAkvaleir = FontsHelper.Pfc.Families.FirstOrDefault(x => x.Name.Contains("Akvaleir")) ?? FontFamily.GenericSansSerif;

        protected Card(string dirPath)
        {
            ResultsDirPath = dirPath + "/results";
            LeftEffectsImage = ImageHelper.LoadImage(CardsDirPath, "left");
            RightEffectsImage = ImageHelper.LoadImage(CardsDirPath, "right");
            PriceImage = ImageHelper.LoadImage(CardsDirPath, "price");
            FrontImage = ImageHelper.LoadImage(CardsDirPath, "front");
            BackgroundImage = ImageHelper.LoadImage(CardsDirPath, "background");
        }

        protected virtual void DrawCard(Graphics graphics)
        {
            if (MainImage != null)
                DrawingHelper.MapDrawing(graphics, MainImage, MainImageArea);
            if (FrontImage != null)
                DrawingHelper.MapDrawing(graphics, FrontImage, FrontImageArea);
            DrawLeftEffectsBackground(graphics);
            DrawRightEffectsBackground(graphics);
            if (Name != null)
            {
                using (var font = new Font(fontAkvaleir, 24, FontStyle.Bold, GraphicsUnit.Pixel))
                    graphics.DrawAdjustedStringWithExtendedBorder(Name.ToUpper(), font, Color.White, Color.Black, NameArea, FontsHelper.StringFormatCentered, 6);
            }
            if (Description != null)
            {
                DrawDescription(graphics);
            }
            if (Type != null)
            {
                using (var font = new Font(FontTrebuchetMs, 12, FontStyle.Regular, GraphicsUnit.Pixel))
                    graphics.DrawAdjustedStringWithExtendedBorder(Type.ToUpper(), font, Color.White, Color.Black, TypeArea, FontsHelper.StringFormatCentered, 6);
            }
            DrawLeftEffects(graphics);
            DrawRightEffects(graphics);
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
                    using (var font = new Font(FontTrebuchetMs, 12, FontStyle.Bold, GraphicsUnit.Pixel))
                        graphics.DrawAdjustedStringWithExtendedBorder(Price.ToString(), font, Color.White, Color.Black, priceImageAreaI, FontsHelper.StringFormatCentered, 6, 12, true, false);
                    priceImageAreaI = new Rectangle(PriceImageArea.X + PriceImageArea.Width, PriceImageArea.Y, PriceImageArea.Width, PriceImageArea.Height);
                    DrawingHelper.MapDrawing(graphics, PriceImage, priceImageAreaI);
                }
            }
            else
            {
                using (var font = new Font(FontTrebuchetMs, 12, FontStyle.Bold, GraphicsUnit.Pixel))
                    graphics.DrawAdjustedStringWithExtendedBorder(Price.ToString(), font, Color.White, Color.Black, PriceImageArea, FontsHelper.StringFormatCentered, 6, 12, true, false);
            }
        }

        protected virtual void DrawLeftEffectsBackground(Graphics graphics)
        {
            if (LeftEffects.Any() && LeftEffectsImage != null)
                DrawingHelper.MapDrawing(graphics, LeftEffectsImage, LeftEffectsImageArea);
        }

        protected virtual void DrawLeftEffects(Graphics graphics)
        {
            var leftEffectsFontSize = GetEffectsFontSize(graphics, FontTrebuchetMs, LeftEffects, LeftEffectsArea);
            for (var i = 0; i < LeftEffects.Count; ++i)
            {
                var effectArea = new Rectangle(LeftEffectsArea.X + LeftEffectsAreaShift.X * i, LeftEffectsArea.Y + LeftEffectsAreaShift.Y * i, LeftEffectsArea.Width, LeftEffectsArea.Height);
                using (var font = new Font(FontTrebuchetMs, leftEffectsFontSize, FontStyle.Bold, GraphicsUnit.Pixel))
                    graphics.DrawAdjustedStringWithExtendedBorder(LeftEffects[i], font, Color.White, Color.Black, effectArea, FontsHelper.StringFormatCentered, 6, (int)leftEffectsFontSize, true, false);
            }
        }

        protected virtual void DrawRightEffectsBackground(Graphics graphics)
        {
            if (RightEffects.Any() && RightEffectsImage != null)
                DrawingHelper.MapDrawing(graphics, RightEffectsImage, RightEffectsImageArea);
        }

        protected virtual void DrawRightEffects(Graphics graphics)
        {
            var rightEffectsFontSize = GetEffectsFontSize(graphics, FontTrebuchetMs, RightEffects, RightEffectsArea);
            for (var i = 0; i < RightEffects.Count; ++i)
            {
                var effectArea = new Rectangle(RightEffectsArea.X + RightEffectsAreaShift.X * i, RightEffectsArea.Y + RightEffectsAreaShift.Y * i, RightEffectsArea.Width, RightEffectsArea.Height);
                using (var font = new Font(FontTrebuchetMs, rightEffectsFontSize, FontStyle.Bold, GraphicsUnit.Pixel))
                    graphics.DrawAdjustedStringWithExtendedBorder(RightEffects[i], font, Color.White, Color.Black, effectArea, FontsHelper.StringFormatCentered, 6, (int)rightEffectsFontSize, true, false);
            }
        }

        protected static float GetEffectsFontSize(Graphics graphics, FontFamily fontFamily, List<string> effects, Rectangle effectArea)
        {
            float effectsFontSize = 20;
            foreach (var effect in effects)
            {
                if (string.IsNullOrEmpty(effect)) continue;

                using (var font = new Font(fontFamily, effectsFontSize, FontStyle.Bold, GraphicsUnit.Pixel))
                    effectsFontSize = FontsHelper.GetAdjustedFont(graphics, effect, font, effectArea, FontsHelper.StringFormatCentered, 6, (int)effectsFontSize, true, false).Size;
            }

            return effectsFontSize;
        }

        protected virtual void DrawDescription(Graphics graphics)
        {
            using (var font = new Font(FontTrebuchetMs, 12, FontStyle.Regular, GraphicsUnit.Pixel))
                graphics.DrawAdjustedStringWithExtendedBorder(DescriptionFull, font, Color.White, Color.Black, DescriptionArea, FontsHelper.StringFormatCentered);
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

        protected virtual void CalculateTypeArea()
        {
            if (Price <= 0) return;

            TypeArea = Price <= PriceLimit
                ? new Rectangle(TypeArea.X + PriceImageArea.Width * Price, TypeArea.Y, TypeArea.Width - PriceImageArea.Width * Price, TypeArea.Height)
                : new Rectangle(TypeArea.X + PriceImageArea.Width * 2, TypeArea.Y, TypeArea.Width - PriceImageArea.Width * 2, TypeArea.Height);
        }
    }
}

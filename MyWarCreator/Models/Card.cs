using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using MyWarCreator.Extensions;
using MyWarCreator.Features.Drawing;
using MyWarCreator.Features.Fonts;
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
        protected Rectangle NameArea { get; set; } = new Rectangle(70, 245, 220, 40);
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
        protected virtual string FileName => string.IsNullOrEmpty(Type) ? $"{Name}" : $"{Type} - {Name}";
        protected int PriceLimit { get; } = 3;

        protected readonly FontFamily FontTrebuchetMs;
        private readonly FontFamily fontAkvaleir;

        private readonly IFontProvider fontProvider;
        private readonly IPainter painter;

        protected Card(IFontProvider fontProvider, IPainter painter, string dirPath)
        {
            this.fontProvider = fontProvider;
            this.painter = painter;
            FontTrebuchetMs = fontProvider.Get("Trebuchet MS");
            fontAkvaleir = fontProvider.Get("Akvaleir");

            ResultsDirPath = dirPath + "/results";
            LeftEffectsImage = ImageHelper.LoadImage(CardsDirPath, "left");
            RightEffectsImage = ImageHelper.LoadImage(CardsDirPath, "right");
            PriceImage = ImageHelper.LoadImage(CardsDirPath, "price");
            FrontImage = ImageHelper.LoadImage(CardsDirPath, "front");
            BackgroundImage = ImageHelper.LoadImage(CardsDirPath, "background");
        }

        protected virtual void DrawCard(Graphics graphics, bool blackAndWhite)
        {
            if (MainImage != null)
                DrawingHelper.MapDrawing(graphics, MainImage, MainImageArea);
            if (FrontImage != null && blackAndWhite == false)
                DrawingHelper.MapDrawing(graphics, FrontImage, FrontImageArea);
            DrawLeftEffectsBackground(graphics);
            DrawRightEffectsBackground(graphics);
            if (Name != null)
            {
                using (var font = new Font(fontAkvaleir, 24, FontStyle.Bold, GraphicsUnit.Pixel))
                    painter.DrawAdjustedStringWithExtendedBorder(graphics, Name.ToUpper(), font, GetColor(blackAndWhite), GetColor(!blackAndWhite), NameArea, FontConsts.StringFormatCentered, 6);
            }
            if (Description != null)
            {
                DrawDescription(graphics, blackAndWhite);
            }
            if (Type != null)
            {
                using (var font = new Font(FontTrebuchetMs, 12, FontStyle.Regular, GraphicsUnit.Pixel))
                    painter.DrawAdjustedStringWithExtendedBorder(graphics, Type.ToUpper(), font, GetColor(blackAndWhite), GetColor(!blackAndWhite), TypeArea, FontConsts.StringFormatCentered, 6);
            }
            DrawLeftEffects(graphics, blackAndWhite);
            DrawRightEffects(graphics, blackAndWhite);
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
                        painter.DrawAdjustedStringWithExtendedBorder(graphics, Price.ToString(), font, GetColor(blackAndWhite), GetColor(!blackAndWhite), priceImageAreaI, FontConsts.StringFormatCentered, 6, 12, true, false);
                    priceImageAreaI = new Rectangle(PriceImageArea.X + PriceImageArea.Width, PriceImageArea.Y, PriceImageArea.Width, PriceImageArea.Height);
                    DrawingHelper.MapDrawing(graphics, PriceImage, priceImageAreaI);
                }
            }
            else
            {
                using (var font = new Font(FontTrebuchetMs, 12, FontStyle.Bold, GraphicsUnit.Pixel))
                    painter.DrawAdjustedStringWithExtendedBorder(graphics, Price.ToString(), font, GetColor(blackAndWhite), GetColor(!blackAndWhite), PriceImageArea, FontConsts.StringFormatCentered, 6, 12, true, false);
            }
        }

        protected virtual void DrawLeftEffectsBackground(Graphics graphics)
        {
            if (LeftEffects.Any() && LeftEffectsImage != null)
                DrawingHelper.MapDrawing(graphics, LeftEffectsImage, LeftEffectsImageArea);
        }

        protected virtual void DrawLeftEffects(Graphics graphics, bool blackAndWhite)
        {
            var leftEffectsFontSize = GetEffectsFontSize(graphics, FontTrebuchetMs, LeftEffects, LeftEffectsArea);
            for (var i = 0; i < LeftEffects.Count; ++i)
            {
                var effectArea = new Rectangle(LeftEffectsArea.X + LeftEffectsAreaShift.X * i, LeftEffectsArea.Y + LeftEffectsAreaShift.Y * i, LeftEffectsArea.Width, LeftEffectsArea.Height);
                using (var font = new Font(FontTrebuchetMs, leftEffectsFontSize, FontStyle.Bold, GraphicsUnit.Pixel))
                    painter.DrawAdjustedStringWithExtendedBorder(graphics, LeftEffects[i], font, GetColor(blackAndWhite), GetColor(!blackAndWhite), effectArea, FontConsts.StringFormatCentered, 6, (int)leftEffectsFontSize, true, false);
            }
        }

        protected virtual void DrawRightEffectsBackground(Graphics graphics)
        {
            if (RightEffects.Any() && RightEffectsImage != null)
                DrawingHelper.MapDrawing(graphics, RightEffectsImage, RightEffectsImageArea);
        }

        protected virtual void DrawRightEffects(Graphics graphics, bool blackAndWhite)
        {
            var rightEffectsFontSize = GetEffectsFontSize(graphics, FontTrebuchetMs, RightEffects, RightEffectsArea);
            for (var i = 0; i < RightEffects.Count; ++i)
            {
                var effectArea = new Rectangle(RightEffectsArea.X + RightEffectsAreaShift.X * i, RightEffectsArea.Y + RightEffectsAreaShift.Y * i, RightEffectsArea.Width, RightEffectsArea.Height);
                using (var font = new Font(FontTrebuchetMs, rightEffectsFontSize, FontStyle.Bold, GraphicsUnit.Pixel))
                    painter.DrawAdjustedStringWithExtendedBorder(graphics, RightEffects[i], font, GetColor(blackAndWhite), GetColor(!blackAndWhite), effectArea, FontConsts.StringFormatCentered, 6, (int)rightEffectsFontSize, true, false);
            }
        }

        protected float GetEffectsFontSize(Graphics graphics, FontFamily fontFamily, IEnumerable<string> effects, Rectangle effectArea)
        {
            float effectsFontSize = 20;
            foreach (var effect in effects)
            {
                if (string.IsNullOrEmpty(effect)) continue;

                using (var font = new Font(fontFamily, effectsFontSize, FontStyle.Bold, GraphicsUnit.Pixel))
                    effectsFontSize = fontProvider.GetAdjusted(graphics, effect, font, effectArea, FontConsts.StringFormatCentered, 6, (int)effectsFontSize, true, false).Size;
            }

            return effectsFontSize;
        }

        protected virtual void DrawDescription(Graphics graphics, bool blackAndWhite)
        {
            using (var font = new Font(FontTrebuchetMs, 12, FontStyle.Regular, GraphicsUnit.Pixel))
                painter.DrawAdjustedStringWithExtendedBorder(graphics, DescriptionFull, font, GetColor(blackAndWhite), GetColor(!blackAndWhite), DescriptionArea, FontConsts.StringFormatCentered);
        }

        public string GenerateFile(string fileNamePrefix = "", string fileNameSuffix = "", bool blackAndWhite = false)
        {
            try
            {
                if (!Directory.Exists(ResultsDirPath))
                {
                    Directory.CreateDirectory(ResultsDirPath);
                }
                var savePath = Path.GetFullPath(ResultsDirPath + "/" + fileNamePrefix + FileName + fileNameSuffix + ".png");
                using (Image bitmap = blackAndWhite ?
                    new Bitmap(BackgroundImage.Width, BackgroundImage.Height) :
                    new Bitmap(BackgroundImage))
                {
                    using (var graphics = Graphics.FromImage(bitmap))
                    {
                        if (blackAndWhite)
                        {
                            graphics.FillRectangle(Brushes.White, 0, 0, BackgroundImage.Width, BackgroundImage.Height);
                            graphics.DrawRectangle(Pens.Black, 0, 0, BackgroundImage.Width - 2, BackgroundImage.Height - 2);
                        }
                        DrawCard(graphics, blackAndWhite);
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

        protected static Color GetColor(bool blackAndWhite) => blackAndWhite ? Color.Black : Color.White;
    }
}

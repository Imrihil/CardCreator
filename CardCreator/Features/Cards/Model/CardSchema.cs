using CardCreator.Features.Images;
using CardCreator.Features.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;

namespace CardCreator.Features.Cards.Model
{
    public class CardSchema : List<ElementSchema>
    {
        public const int HeaderRows = 5;
        private const int BackgroundIdx = 0;
        private const int WidthPxIdx = 1;
        private const int HeightPxIdx = 2;
        private const int WidthInchIdx = 3;
        private const int HeightInchIdx = 4;

        public Image Background { get; }
        public int WidthPx { get; }
        public int HeightPx { get; }
        public double WidthInch { get; }
        public double HeightInch { get; }

        public CardSchema(ILogger logger, IImageProvider imageProvider, IList<string> parameters) : base(new List<ElementSchema>())
        {
            if (!string.IsNullOrEmpty(parameters[BackgroundIdx]))
                Background = imageProvider.Get(parameters[BackgroundIdx]);
            try
            {
                WidthPx = int.Parse(parameters[WidthPxIdx]);
                if (WidthPx <= 0) throw new FormatException($"{WidthPxIdx + 1}nd parameter must be a positive integer, but \"{parameters[WidthPxIdx]}\" is not.");
            }
            catch (FormatException)
            {
                logger.LogMessage($"{WidthPxIdx + 1}. parametr musi być dodatnią liczbą całkowitą, a \"{parameters[WidthPxIdx]}\" nie jest.");
            }
            try
            {
                HeightPx = int.Parse(parameters[HeightPxIdx]);
                if (HeightPx <= 0) throw new FormatException($"{HeightPxIdx + 1}rd parameter must be a positive integer, but \"{parameters[HeightPxIdx]}\" is not.");
            }
            catch (FormatException)
            {
                logger.LogMessage($"{HeightPxIdx + 1}. parametr musi być dodatnią liczbą całkowitą, a \"{parameters[WidthPxIdx]}\" nie jest.");
            }
            try
            {
                WidthInch = double.Parse(parameters[WidthInchIdx].Replace(',', '.'), CultureInfo.InvariantCulture);
                if (WidthInch <= 0) throw new FormatException($"{WidthInchIdx + 1}th parameter must be a positive number, but \"{parameters[WidthInchIdx]}\" is not.");
            }
            catch (FormatException)
            {
                logger.LogMessage($"{WidthInchIdx + 1}. parametr musi być liczbą dodatnią, a \"{parameters[WidthPxIdx]}\" nie jest.");
            }
            try
            {
                HeightInch = double.Parse(parameters[HeightInchIdx].Replace(',', '.'), CultureInfo.InvariantCulture);
                if (HeightInch <= 0) throw new FormatException($"{HeightInchIdx + 1}th parameter must be a positive number, but \"{parameters[HeightInchIdx]}\" is not.");
            }
            catch (FormatException)
            {
                logger.LogMessage($"{HeightInchIdx + 1}. parametr musi być liczbą dodatnią, a \"{parameters[WidthPxIdx]}\" nie jest.");
            }
        }
    }
}

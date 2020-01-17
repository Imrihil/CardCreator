using PdfSharp;
using System.Collections.Generic;
using System.Drawing;

namespace CardCreator.Settings
{
    public class AppSettings
    {
        public const double PointsInInch = 72;

        public IEnumerable<ButtonSettings> Buttons { get; set; }
        public PageSettings Page { get; set; }
        public TextSettings Text { get; set; }
        public string CardsDirectory { get; set; } = "cards";
        public int Dpi { get; set; } = 150;
        public int ImageCacheTimeout { get; set; } = 10;
    }
}

using PdfSharp;
using System.Collections.Generic;
using System.Drawing;

namespace CardCreator.Settings
{
    public class AppSettings
    {
        public const double PointsInInch = 72;

        public IEnumerable<ButtonSettings> Buttons { get; set; }
        public string CardsDirectory { get; set; } = "cards";
        public int Dpi { get; set; } = 150;
        public PageSize PageSize { get; set; } = PageSize.A4;
        public PageOrientation PageOrientation { get; set; } = PageOrientation.Portrait;
        public double PageWidthInch =>
            PageOrientation == PageOrientation.Portrait ?
            PageSizeInch[PageSize].X :
            PageSizeInch[PageSize].Y;
        public double PageHeightInch =>
            PageOrientation == PageOrientation.Portrait ?
            PageSizeInch[PageSize].Y :
            PageSizeInch[PageSize].X;
        public double PageMarginPts { get; set; } = 20;
        public double CardsMarginPts { get; set; } = 1;
        public int ImageCacheTimeout { get; set; } = 10;

        private readonly Dictionary<PageSize, PointF> PageSizeInch = new Dictionary<PageSize, PointF>
        {
            {PageSize.A0, new PointF((float)33.1, (float)46.8) },
            {PageSize.A1, new PointF((float)23.4, (float)33.1) },
            {PageSize.A2, new PointF((float)16.5, (float)23.4) },
            {PageSize.A3, new PointF((float)11.7, (float)16.5) },
            {PageSize.A4, new PointF((float)8.3, (float)11.7) },
            {PageSize.A5, new PointF((float)5.8, (float)8.3) },
        };
    }
}

using PdfSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace CardCreator.Settings
{
    public class PageSettings
    {
        public PageSize Size { get; set; } = PageSize.A4;
        public PageOrientation Orientation { get; set; } = PageOrientation.Portrait;
        public double WidthInch =>
            Orientation == PageOrientation.Portrait ?
            PageSizeInch[Size].X :
            PageSizeInch[Size].Y;
        public double HeightInch =>
            Orientation == PageOrientation.Portrait ?
            PageSizeInch[Size].Y :
            PageSizeInch[Size].X;
        public double MarginPts { get; set; } = 20;
        public double CardsMarginPts { get; set; } = 1;

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

using System.Drawing;

namespace MyWarCreator.Features.Fonts
{
    public class FontConsts
    {

        public static readonly StringFormat StringFormatLeft = new StringFormat
        {
            Alignment = StringAlignment.Near,
            LineAlignment = StringAlignment.Center
        };

        public static readonly StringFormat StringFormatCentered = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        public static readonly StringFormat StringFormatRight = new StringFormat
        {
            Alignment = StringAlignment.Far,
            LineAlignment = StringAlignment.Center
        };
    }
}

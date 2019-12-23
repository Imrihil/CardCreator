using System;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;

namespace MyWarCreator.Helpers
{
    public class FontsHelper
    {
        static FontsHelper()
        {
            StringFormatLeft = new StringFormat
            {
                Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center
            };
            StringFormatCentered = new StringFormat
            {
                Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center
            };
            StringFormatRight = new StringFormat
            {
                Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center
            };
        }
        public static PrivateFontCollection Pfc { get; } = new PrivateFontCollection();

        public static StringFormat StringFormatLeft { get; }
        public static StringFormat StringFormatCentered { get; }
        public static StringFormat StringFormatRight { get; }

        public static void AddFont(byte[] font)
        {
            //Select your font from the resources.
            var fontLength = font.Length;
            // create a buffer to read in to
            var fontData = font;
            // create an unsafe memory block for the font data
            var data = Marshal.AllocCoTaskMem(fontLength);
            // copy the bytes to the unsafe memory block
            Marshal.Copy(fontData, 0, data, fontLength);
            // pass the font to the font collection
            Pfc.AddMemoryFont(data, fontLength);
        }

        public static Font GetAdjustedFont(Graphics graphicRef, string graphicString, Font originalFont, RectangleF container, StringFormat stringFormat, int minFontSize, int maxFontSize, bool smallestOnFail = true, bool wordWrap = true)
        {
            // We utilize MeasureString which we get via a control instance           
            for (var adjustedSize = maxFontSize; adjustedSize >= minFontSize; adjustedSize--)
            {
                var testFont = new Font(originalFont.Name, adjustedSize, originalFont.Style);

                // Test the string with the new size
                var adjustedSizeNew = graphicRef.MeasureString(graphicString, testFont, new SizeF(container.Width, container.Height), stringFormat, out var characterFitted, out var linesFilled);

                if (characterFitted == graphicString.Length && (wordWrap || linesFilled == graphicString.Count(x => x == '\n') + 1) && container.Width > Convert.ToInt32(adjustedSizeNew.Width) && container.Height > Convert.ToInt32(adjustedSizeNew.Height))
                {
                    // Good font, return it
                    return testFont;
                }
            }

            // If you get here there was no font size that worked
            // return MinimumSize or Original?
            return smallestOnFail
                ? new Font(originalFont.Name, minFontSize, originalFont.Style)
                : originalFont;
        }
    }
}

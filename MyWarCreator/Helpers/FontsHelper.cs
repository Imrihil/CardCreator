using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MyWarCreator.Helpers
{
    class FontsHelper
    {
        static FontsHelper()
        {
            StringFormatCentered = new StringFormat();
            StringFormatCentered.Alignment = StringAlignment.Center;
            StringFormatCentered.LineAlignment = StringAlignment.Center;
        }
        public static PrivateFontCollection pfc { get; } = new PrivateFontCollection();

        public static StringFormat StringFormatCentered { get; private set; }

        public static void AddFont(byte[] font)
        {
            //Select your font from the resources.
            int fontLength = font.Length;
            // create a buffer to read in to
            byte[] fontdata = font;
            // create an unsafe memory block for the font data
            System.IntPtr data = Marshal.AllocCoTaskMem(fontLength);
            // copy the bytes to the unsafe memory block
            Marshal.Copy(fontdata, 0, data, fontLength);
            // pass the font to the font collection
            pfc.AddMemoryFont(data, fontLength);
        }

        public static Font GetAdjustedFont(Graphics graphicRef, string graphicString, Font originalFont, RectangleF container, int minFontSize, int maxFontSize, bool smallestOnFail = true)
        {
            // We utilize MeasureString which we get via a control instance           
            for (int AdjustedSize = maxFontSize; AdjustedSize >= minFontSize; AdjustedSize--)
            {
                Font TestFont = new Font(originalFont.Name, AdjustedSize, originalFont.Style);

                // Test the string with the new size
                SizeF AdjustedSizeNew = graphicRef.MeasureString(graphicString, TestFont);

                if (container.Width > Convert.ToInt32(AdjustedSizeNew.Width) && container.Height > Convert.ToInt32(AdjustedSizeNew.Height))
                {
                    // Good font, return it
                    return TestFont;
                }
            }

            // If you get here there was no fontsize that worked
            // return MinimumSize or Original?
            if (smallestOnFail)
            {
                return new Font(originalFont.Name, minFontSize, originalFont.Style);
            }
            else
            {
                return originalFont;
            }
        }
    }
}

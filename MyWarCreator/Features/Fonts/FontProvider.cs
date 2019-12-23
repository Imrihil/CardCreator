using System;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;

namespace MyWarCreator.Features.Fonts
{
    public class FontProvider : IFontProvider
    {
        private readonly PrivateFontCollection pfc = new PrivateFontCollection();
        private readonly InstalledFontCollection ifc = new InstalledFontCollection();

        public void Register(byte[] font)
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
            pfc.AddMemoryFont(data, fontLength);
        }

        public Font GetAdjusted(Graphics graphicRef, string graphicString, Font originalFont, RectangleF container, StringFormat stringFormat, int minFontSize, int maxFontSize, bool smallestOnFail = true, bool wordWrap = true)
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

        public FontFamily Get(string name) =>
            pfc.Families.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)) ??
            ifc.Families.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)) ??
            FontFamily.GenericSansSerif;
    }
}

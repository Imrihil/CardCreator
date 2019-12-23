using System.Drawing;

namespace CardCreator.Features.Fonts
{
    public interface IFontProvider
    {
        void Register(byte[] font);
        FontFamily Get(string name);
        Font GetAdjusted(Graphics graphicRef, string graphicString, Font originalFont, RectangleF container, StringFormat stringFormat, int minFontSize, int maxFontSize, bool smallestOnFail = true, bool wordWrap = true);
    }
}

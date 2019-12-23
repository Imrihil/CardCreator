using System.Drawing;

namespace MyWarCreator.Features.Drawing
{
    public interface IPainter
    {
        void DrawStringWithShadow(Graphics graphics, string s, Font font, Color color, RectangleF layoutRectangle, int shadowSize);
        void DrawStringWithShadow(Graphics graphics, string s, Font font, Color color, RectangleF layoutRectangle, StringFormat format, int shadowSize);
        void DrawAdjustedString(Graphics graphics, string s, Font font, Brush brush, RectangleF layoutRectangle, int minFontSize = 0, int maxFontSize = int.MinValue, bool smallestOnFail = true, bool wordWrap = true);
        void DrawAdjustedString(Graphics graphics, string s, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format, int minFontSize = 0, int maxFontSize = int.MinValue, bool smallestOnFail = true, bool wordWrap = true);
        void DrawAdjustedStringWithBorder(Graphics graphics, string s, Font font, Brush brush, Brush brushBorder, RectangleF layoutRectangle, StringFormat format, int minFontSize = 0, int maxFontSize = int.MinValue, bool smallestOnFail = true, bool wordWrap = true);
        void DrawAdjustedStringWithExtendedBorder(Graphics graphics, string s, Font font, Color color, Color colorBorder, RectangleF layoutRectangle, StringFormat format, int minFontSize = 0, int maxFontSize = int.MinValue, bool smallestOnFail = true, bool wordWrap = true);
    }
}

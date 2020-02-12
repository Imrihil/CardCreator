using System.Drawing;

namespace CardCreator.Features.Drawing
{
    public interface IImageProvider
    {
        Image Get(string name);
        Image TryGet(string name);
        Image TryGetImageFromColor(string name, string widthStr, string heightStr);
        Image TryGetImageFromColor(string name, int width, int height);
    }
}
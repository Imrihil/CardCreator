using System.Drawing;

namespace CardCreator.Features.Drawing
{
    public interface IImageProvider
    {
        Image Get(string name);
        Image TryGet(string name);
    }
}
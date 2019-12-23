using System.Drawing;

namespace CardCreator.Features.Images
{
    public interface IImageProvider
    {
        Image Get(string name);
    }
}
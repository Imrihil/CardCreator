using System.Drawing;

namespace MyWarCreator.Features.Images
{
    public interface IImageProvider
    {
        Image Get(string name);
    }
}
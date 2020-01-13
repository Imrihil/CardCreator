using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CardCreator.Features.Preview
{
    public interface IPreview
    {
        bool GenerateImages { get; }
        Task<BitmapImage> GetImage();
        Task<BitmapImage> Next();
        Task<BitmapImage> Previous();
        Task Refresh(bool generateImages);
    }
}

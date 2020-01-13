using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CardCreator.Features.Preview
{
    public interface IPreview
    {
        bool GenerateImages { get; }
        Task<BitmapImage> GetImage(int gridWidth, int gridHeight);
        Task<BitmapImage> Next(int gridWidth, int gridHeight);
        Task<BitmapImage> Previous(int gridWidth, int gridHeight);
        Task Refresh(bool generateImages);
    }
}

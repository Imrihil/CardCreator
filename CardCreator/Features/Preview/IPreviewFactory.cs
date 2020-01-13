using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CardCreator.Features.Preview
{
    public interface IPreviewFactory
    {
        Task<BitmapImage> GetPreviewImage(int gridWidth, int gridHeight);
        Task<BitmapImage> NextPreviewImage(int gridWidth, int gridHeight);
        Task<BitmapImage> PreviousPreviewImage(int gridWidth, int gridHeight);
        Task Refresh(bool generateImages);
        Task<string> Register(string filePath, bool generateImages);
        Task<bool> Register(string key, string filePath, bool generateImages);
        Task SetCurrentPreview(string key, bool generateImages);
    }
}

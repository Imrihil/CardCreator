using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CardCreator.Features.Preview
{
    public interface IPreviewFactory
    {
        string Register(string filePath, bool generateImages);
        bool Register(string key, string filePath, bool generateImages);
        Task Refresh(bool generateImages);
        Task SetCurrentPreview(string key, bool generateImages);
        Task<BitmapImage> GetPreviewImage();
        Task<BitmapImage> NextPreviewImage();
        Task<BitmapImage> PreviousPreviewImage();
    }
}

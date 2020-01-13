using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;

namespace CardCreator.Features.Preview
{
    public interface IPreviewFactory
    {
        string Register(string filePath);
        bool Register(string key, string filePath);
        void SetCurrentPreview(string key);
        BitmapImage GetPreviewImage();
        BitmapImage NextPreviewImage();
        BitmapImage PreviousPreviewImage();
    }
}

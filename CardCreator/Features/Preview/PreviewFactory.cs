using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;

namespace CardCreator.Features.Preview
{
    public class PreviewFactory : IPreviewFactory
    {
        private readonly Dictionary<string, IPreview> previews;
        private IPreview CurrentPreview { get; set; }
        private BitmapImage LastPreview { get; set; }

        public PreviewFactory()
        {
            previews = new Dictionary<string, IPreview>();
        }

        public BitmapImage GetPreviewImage() =>
            UpdateLastPreview(CurrentPreview.GetImage());

        public BitmapImage NextPreviewImage() =>
            UpdateLastPreview(CurrentPreview.Next());

        public BitmapImage PreviousPreviewImage() =>
            UpdateLastPreview(CurrentPreview.Previous());

        private BitmapImage UpdateLastPreview(BitmapImage bitmapImage)
        {
            if (LastPreview != null)
                LastPreview.StreamSource.Dispose();
            LastPreview = bitmapImage;
            return LastPreview;
        }

        public string Register(string filePath)
        {
            var key = previews.Count.ToString();
            previews.Add(key, new Preview(filePath));
            return key;
        }

        public bool Register(string key, string filePath)
        {
            if (previews.ContainsKey(key))
            {
                previews[key] = new Preview(filePath);
                return true;
            }
            previews.Add(key, new Preview(filePath));
            return false;
        }

        public void SetCurrentPreview(string key) =>
            CurrentPreview = previews[key];
    }
}

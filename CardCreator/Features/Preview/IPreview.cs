using System.Windows.Media.Imaging;

namespace CardCreator.Features.Preview
{
    public interface IPreview
    {
        BitmapImage GetImage();
        BitmapImage Next();
        BitmapImage Previous();
    }
}

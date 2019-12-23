using System.Globalization;
using System.Windows;
using MyWarCreator.Features.Fonts;

namespace MyWarCreator
{
    // https://www.codeproject.com/Articles/299436/WPF-Localization-for-Dummies
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private IFontProvider fontProvider;

        private bool IsBlackAndWhiteChecked => BlackAndWhiteCheckbox.IsChecked ?? false;
        private double CardWidth => double.TryParse(CardWidthTextBox.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out var dec) ? dec : 2.5;
        private double CardHeight => double.TryParse(CardHeightTextBox.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out var dec) ? dec : 3.5;

        public MainWindow(IFontProvider fontProvider)
        {
            this.fontProvider = fontProvider;

            InitializeComponent();
            InitializeFonts();
        }

        private void InitializeFonts()
        {
            fontProvider.Register(Properties.Resources.Akvaléir_Normal_v2007);
            fontProvider.Register(Properties.Resources.colonna_mt);
            fontProvider.Register(Properties.Resources.runic);
            fontProvider.Register(Properties.Resources.runic_altno);
            fontProvider.Register(Properties.Resources.trebuc);
            fontProvider.Register(Properties.Resources.trebucbd);
            fontProvider.Register(Properties.Resources.trebucbi);
            fontProvider.Register(Properties.Resources.trebucit);
        }

        private void ButtonGenerateCards_Click(object sender, RoutedEventArgs e)
        { }

        private void ButtonPreparePdf_Click(object sender, RoutedEventArgs e)
        { }
    }
}

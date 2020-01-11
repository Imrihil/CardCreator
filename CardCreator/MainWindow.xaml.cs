using System.IO;
using System.Windows;
using Microsoft.Win32;
using CardCreator.Features.Cards;
using CardCreator.Features.Drawing;
using CardCreator.Features.Fonts;
using CardCreator.Features.Images;
using MediatR;
using System;
using System.ComponentModel;
using System.Threading;
using CardCreator.Settings;
using Microsoft.Extensions.Options;
using System.Windows.Controls;
using System.Linq;

namespace CardCreator
{
    // https://www.codeproject.com/Articles/299436/WPF-Localization-for-Dummies
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private const int RowHeight = 30;

        private readonly AppSettings settings;
        private readonly IMediator mediator;
        private readonly IFontProvider fontProvider;
        private readonly IImageProvider imageProvider;

        private OpenFileDialog ChooseFileDialog { get; }
        private OpenFileDialog ChooseImagesDialog { get; }

        public MainWindow(IOptions<AppSettings> settings, IMediator mediator, IFontProvider fontProvider, IImageProvider imageProvider)
        {
            this.settings = settings.Value;
            this.mediator = mediator;
            this.fontProvider = fontProvider;
            this.imageProvider = imageProvider;

            InitializeComponent();

            ChooseFileDialog = InitializeChooseFileDialog();
            ChooseImagesDialog = InitializeChooseImagesDialog();
            InitializeFonts();
            InitializeControls();
            InitializeButtons();
        }

        private OpenFileDialog InitializeChooseFileDialog()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Excel files (*.xls;*xlsx)|*.xls;*xlsx",
                Title = Properties.Resources.ResourceManager.GetString("ChooseFile"),
#if DEBUG
                InitialDirectory = Path.GetFullPath(Directory.GetCurrentDirectory() + "../../../../../AppData")
#else
                InitialDirectory = Path.GetFullPath(Directory.GetCurrentDirectory())
#endif
            };
            return openFileDialog;
        }

        private OpenFileDialog InitializeChooseImagesDialog()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter =
                    "Images (*.bmp;*.jpg;*.png;*.gif)|*.bmp;*.jpg;*.png;*.gif|" +
                    "All files (*.*)|*.*",
                Multiselect = true,
                Title = Properties.Resources.ResourceManager.GetString("ChooseImages"),
#if DEBUG
                InitialDirectory = Path.GetFullPath(Directory.GetCurrentDirectory() + "../../../../../AppData")
#else
                InitialDirectory = Path.GetFullPath(Directory.GetCurrentDirectory())
#endif
            };
            return openFileDialog;
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

        private void InitializeControls()
        {
            GenerateCards_Button.IsEnabled = !string.IsNullOrEmpty(ChooseFileDialog.FileName);
            PreparePdf_Button.IsEnabled = !string.IsNullOrEmpty(ChooseFileDialog.FileName);
            PrepareChoosenPdf_Button.IsEnabled = !string.IsNullOrEmpty(ChooseFileDialog.FileName);
            Dpi_TextBox.Text = settings.Dpi.ToString();
        }

        private void InitializeButtons()
        {
            var currentRow = MainGrid.RowDefinitions.Count - 1;
            foreach (var button in settings.Buttons)
            {
                currentRow = NewButtonRow(currentRow);

                if (!string.IsNullOrEmpty(button.Generate))
                    InitializeButton(button, ButtonAction.Generate, button.Generate, currentRow);
                if (!string.IsNullOrEmpty(button.Pdf))
                    InitializeButton(button, ButtonAction.Pdf, button.Pdf, currentRow);

                currentRow++;
            }
        }

        private int NewButtonRow(int currentRow)
        {
            Application.Current.MainWindow.Height += RowHeight;
            MainGrid.RowDefinitions.Insert(currentRow, new RowDefinition { Height = new GridLength(RowHeight) });
            return currentRow;
        }

        private void InitializeButton(ButtonSettings button, ButtonAction action, string content, int row)
        {
            var control = new Button
            {
                Content = content,
                Margin = new Thickness(5, 5, 5, 5),
                IsEnabled = File.Exists(button.File)
            };

            control.Click += GetAction(button, action);

            Grid.SetRow(control, row);
            Grid.SetColumn(control, GetColumnNumber(action));
            Grid.SetColumnSpan(control, 2);

            MainGrid.Children.Add(control);
        }

        private int GetColumnNumber(ButtonAction action)
        {
            return action switch
            {
                ButtonAction.Generate => 1,
                ButtonAction.Pdf => 3,
                _ => 0,
            };
        }

        private RoutedEventHandler GetAction(ButtonSettings button, ButtonAction action)
        {
            return action switch
            {
                ButtonAction.Generate => new RoutedEventHandler((sender, e) => GenerateCard(button.File)),
                ButtonAction.Pdf => new RoutedEventHandler((sender, e) => PreparePdf(button.File)),
                _ => null,
            };
        }

        private void ChooseFile_Button_Click(object sender, RoutedEventArgs e)
        {
            if (ChooseFileDialog.ShowDialog() == true)
            {
                var fileInfo = new FileInfo(ChooseFileDialog.FileName);
                ChoosenFile_Label.Content = fileInfo.Name;
                ChooseFileDialog.InitialDirectory = fileInfo.Directory.FullName;

                GenerateCards_Button.IsEnabled = !string.IsNullOrEmpty(ChooseFileDialog.FileName);
                PreparePdf_Button.IsEnabled = !string.IsNullOrEmpty(ChooseFileDialog.FileName);
            }
        }

        private void GenerateCards_Button_Click(object sender, RoutedEventArgs e)
        {
            GenerateCard(ChooseFileDialog.FileName);
        }

        private void PreparePdf_Button_Click(object sender, RoutedEventArgs e)
        {
            PreparePdf(ChooseFileDialog.FileName);
        }

        private void GenerateCard(string fileName)
        {
            var cts = new CancellationTokenSource();
            var result = mediator.Send(new CardGeneratingCommand(fileName, GenerateImages_Checkbox.IsChecked ?? true, cts), cts.Token).GetAwaiter().GetResult();
            Console.WriteLine(result);
        }

        private void PreparePdf(string fileName)
        {
            var cts = new CancellationTokenSource();
            var result = mediator.Send(new PdfGeneratingCommand(fileName, GenerateImages_Checkbox.IsChecked ?? true, cts), cts.Token).GetAwaiter().GetResult();
            Console.WriteLine(result);
        }

        private void ChooseImages_Button_Click(object sender, RoutedEventArgs e)
        {
            if (ChooseImagesDialog.ShowDialog() == true)
            {
                var file = ChooseImagesDialog.FileNames.FirstOrDefault();
                if (file == null) return;

                var fileInfo = new FileInfo(file);
                ChooseImagesDialog.InitialDirectory = fileInfo.Directory.FullName;

                ChoosenImages_Label.Content = $"{Properties.Resources.ResourceManager.GetString("Choosen")} {ChooseImagesDialog.FileNames.Count()}";
                PrepareChoosenPdf_Button.IsEnabled = ChooseImagesDialog.FileNames.Any();
            }
        }

        private void PrepareChoosenPdf_Button_Click(object sender, RoutedEventArgs e)
        {
            var cts = new CancellationTokenSource();
            if (int.TryParse(Dpi_TextBox.Text, out var dpi))
            {
                var result = mediator.Send(new PdfGeneratingFromImagesCommand(ChooseImagesDialog.FileNames, dpi, cts), cts.Token).GetAwaiter().GetResult();
                Console.WriteLine(result);
            }
            else
            {
                MessageBox.Show($"{Dpi_TextBox.Text} is not a valid integer", $"Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            Environment.Exit(0);
        }
    }
}

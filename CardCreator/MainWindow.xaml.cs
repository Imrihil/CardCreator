using CardCreator.Features.Cards;
using CardCreator.Features.Fonts;
using CardCreator.Features.Preview;
using CardCreator.Settings;
using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace CardCreator
{
    // https://www.codeproject.com/Articles/299436/WPF-Localization-for-Dummies
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private const int RowHeight = 30;
        private const string ChoosenFile = "ChoosenFile";

        private readonly AppSettings settings;
        private readonly IMediator mediator;
        private readonly IFontProvider fontProvider;
        private readonly IPreviewFactory previewFactory;

        private OpenFileDialog ChooseFileDialog { get; }
        private OpenFileDialog ChooseImagesDialog { get; }
        private DispatcherTimer PreviewTimer { get; }
        private bool GenerateImages => GenerateImages_Checkbox.IsChecked ?? true;
        private int GridWidth => IntParse(GridWidth_TextBox.Text, out var gridWidth) ? gridWidth : 0;
        private int GridHeight => IntParse(GridHeight_TextBox.Text, out var gridHeight) ? gridHeight : 0;

        public MainWindow(IOptions<AppSettings> settings, IMediator mediator, IFontProvider fontProvider, IPreviewFactory previewFactory)
        {
            this.settings = settings.Value;
            this.mediator = mediator;
            this.fontProvider = fontProvider;
            this.previewFactory = previewFactory;

            InitializeComponent();

            ChooseFileDialog = InitializeChooseFileDialog();
            ChooseImagesDialog = InitializeChooseImagesDialog();
            PreviewTimer = InitializePreviewTimer();
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

        private DispatcherTimer InitializePreviewTimer()
        {
            var timer = new DispatcherTimer();
            timer.Tick += new EventHandler((sender, e) => RefreshPreview());
            timer.Interval = new TimeSpan(0, 0, 1);
            return timer;
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
            Preview_RadioButton_ChoosenFile.IsEnabled = !string.IsNullOrEmpty(ChooseFileDialog.FileName);
            PreviewAutoRefresh_Checkbox.Visibility = Preview_Image.Source == null ? Visibility.Hidden : Visibility.Visible;
            PreviousPreview_Button.Visibility = Preview_Image.Source == null ? Visibility.Hidden : Visibility.Visible;
            NextPreview_Button.Visibility = Preview_Image.Source == null ? Visibility.Hidden : Visibility.Visible;
        }

        private void InitializeButtons()
        {
            var currentRow = MainGrid.RowDefinitions.Count - 1;
            var isFirst = true;
            foreach (var button in settings.Buttons)
            {
                currentRow = NewButtonRow(currentRow, isFirst);

                if (!string.IsNullOrEmpty(button.Generate))
                    InitializeButton(button, ButtonAction.Generate, button.Generate, currentRow);
                if (!string.IsNullOrEmpty(button.Pdf))
                    InitializeButton(button, ButtonAction.Pdf, button.Pdf, currentRow);
                InitializePreviewRadioButton(button, currentRow, isFirst);

                isFirst = false;
                currentRow++;
            }
        }

        private int NewButtonRow(int currentRow, bool withSeparator)
        {
            Application.Current.MainWindow.Height += RowHeight;
            Grid.SetRowSpan(Vertical_Rectangle, Grid.GetRowSpan(Vertical_Rectangle) + (withSeparator ? 2 : 1));
            Grid.SetRowSpan(Preview_Image, Grid.GetRowSpan(Preview_Image) + (withSeparator ? 2 : 1));
            Grid.SetRow(PreviousPreview_Button, Grid.GetRow(PreviousPreview_Button) + (withSeparator ? 2 : 1));
            Grid.SetRow(NextPreview_Button, Grid.GetRow(NextPreview_Button) + (withSeparator ? 2 : 1));
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

        private void InitializePreviewRadioButton(ButtonSettings button, int row, bool isChecked)
        {
            var name = previewFactory.Register(button.File, GenerateImages).GetAwaiter().GetResult();
            var control = new RadioButton
            {
                Name = $"Preview_RadioButton_{name}",
                GroupName = "Preview",
                IsChecked = isChecked,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            control.Click += new RoutedEventHandler((sender, e) =>
            {
                previewFactory.SetCurrentPreview(name, GenerateImages).GetAwaiter().GetResult();
                Preview_Image.Source = previewFactory.GetPreviewImage(GridWidth, GridHeight).GetAwaiter().GetResult();
                UpdatePreviewTimer();
            });

            Grid.SetRow(control, row);
            Grid.SetColumn(control, 5);

            MainGrid.Children.Add(control);

            if (isChecked)
            {
                PreviewAutoRefresh_Checkbox.Visibility = Visibility.Visible;
                PreviousPreview_Button.Visibility = Visibility.Visible;
                NextPreview_Button.Visibility = Visibility.Visible;
                previewFactory.SetCurrentPreview(name, GenerateImages).GetAwaiter().GetResult();
                Preview_Image.Source = previewFactory.GetPreviewImage(GridWidth, GridHeight).GetAwaiter().GetResult();
                UpdatePreviewTimer();
            }
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
                previewFactory.Register(ChoosenFile, ChooseFileDialog.FileName, GenerateImages).GetAwaiter().GetResult();
                Preview_RadioButton_ChoosenFile.IsEnabled = true;
            }
            else
            {
                Preview_RadioButton_ChoosenFile.IsEnabled = false;
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
            var result = mediator.Send(new CardGeneratingCommand(fileName, GenerateImages, cts), cts.Token).GetAwaiter().GetResult();
            Console.WriteLine(result);
        }

        private void PreparePdf(string fileName)
        {
            var cts = new CancellationTokenSource();
            var result = mediator.Send(new PdfGeneratingCommand(fileName, GenerateImages, cts), cts.Token).GetAwaiter().GetResult();
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
            if (IntParse(Dpi_TextBox.Text, out var dpi))
            {
                var result = mediator.Send(new PdfGeneratingFromImagesCommand(ChooseImagesDialog.FileNames, dpi, cts), cts.Token).GetAwaiter().GetResult();
                Console.WriteLine(result);
            }
        }

        private bool IntParse(string text, out int value)
        {
            if (int.TryParse(text, out value))
                return true;

            MessageBox.Show($"{Dpi_TextBox.Text} is not a valid integer", $"Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        private void PreviousPreview_Button_Click(object sender, RoutedEventArgs e)
        {
            Preview_Image.Source = previewFactory.PreviousPreviewImage(GridWidth, GridHeight).GetAwaiter().GetResult();
            UpdatePreviewTimer();
        }

        private void NextPreview_Button_Click(object sender, RoutedEventArgs e)
        {
            Preview_Image.Source = previewFactory.NextPreviewImage(GridWidth, GridHeight).GetAwaiter().GetResult();
            UpdatePreviewTimer();
        }

        private void Preview_RadioButton_ChoosenFile_Click(object sender, RoutedEventArgs e)
        {
            previewFactory.SetCurrentPreview(ChoosenFile, GenerateImages).GetAwaiter().GetResult();
            Preview_Image.Source = previewFactory.GetPreviewImage(GridWidth, GridHeight).GetAwaiter().GetResult();
            UpdatePreviewTimer();
        }

        private void GenerateImages_Checkbox_Click(object sender, RoutedEventArgs e) =>
            RefreshPreview();

        private void Grid_TextBox_LostFocus(object sender, RoutedEventArgs e) =>
            RefreshPreview();

        private void RefreshPreview()
        {
            previewFactory.Refresh(GenerateImages).GetAwaiter().GetResult();
            Preview_Image.Source = previewFactory.GetPreviewImage(GridWidth, GridHeight).GetAwaiter().GetResult();
        }

        private void PreviewAutoRefresh_Checkbox_Click(object sender, RoutedEventArgs e)
        {
            UpdatePreviewTimer();
        }

        private void UpdatePreviewTimer()
        {
            if (PreviewAutoRefresh_Checkbox.IsChecked == true)
            {
                if (!PreviewTimer.IsEnabled)
                    PreviewTimer.Start();
            }
            else if (PreviewTimer.IsEnabled)
                PreviewTimer.Stop();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            Environment.Exit(0);
        }
    }
}

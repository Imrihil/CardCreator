using CardCreator.Features.Cards;
using CardCreator.Features.Fonts;
using CardCreator.Features.Preview;
using CardCreator.Features.Threading;
using CardCreator.Settings;
using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Input;
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
        private List<RadioButton> PreviewRadioButtons { get; }
        private bool GenerateImages => GenerateImages_Checkbox.IsChecked ?? true;
        private int GridWidth => IntParse(GridWidth_TextBox.Text, out var gridWidth) ? gridWidth : 0;
        private int GridHeight => IntParse(GridHeight_TextBox.Text, out var gridHeight) ? gridHeight : 0;

        public MainWindow(IOptions<AppSettings> settings, IMediator mediator, IFontProvider fontProvider, IPreviewFactory previewFactory)
        {
            this.settings = settings.Value;
            this.mediator = mediator;
            this.fontProvider = fontProvider;
            this.previewFactory = previewFactory;

            InitializeLanguage(settings.Value);
            InitializeComponent();

            PreviewRadioButtons = new List<RadioButton>(new[] { Preview_RadioButton_ChoosenFile });

            ChooseFileDialog = InitializeChooseFileDialog();
            ChooseImagesDialog = InitializeChooseImagesDialog();
            PreviewTimer = InitializePreviewTimer();
            InitializeFonts();
            InitializeControls();
            InitializeButtons();
        }

        private void InitializeLanguage(AppSettings settings)
        {
            if (settings.Language == null) 
                return;

            try
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(settings.Language);
            }
            catch { }
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
            UpdatePreviewControls(null);
        }

        private void InitializeButtons()
        {
            if (settings.Buttons == null) return;

            var currentRow = MainGrid.RowDefinitions.Count - 2;
            var isFirst = true;
            var isChecked = true;
            foreach (var button in settings.Buttons)
            {
                currentRow = NewButtonRow(currentRow, isFirst);

                if (!string.IsNullOrEmpty(button.Generate))
                    InitializeButton(button, ButtonAction.Generate, button.Generate, currentRow);
                if (!string.IsNullOrEmpty(button.Pdf))
                    InitializeButton(button, ButtonAction.Pdf, button.Pdf, currentRow);
                InitializePreviewRadioButton(button, currentRow, isChecked && File.Exists(button.File));

                isFirst = false;
                if (isChecked && File.Exists(button.File))
                    isChecked = false;
                currentRow++;
            }
        }

        private int NewButtonRow(int currentRow, bool withSeparator)
        {
            Application.Current.MainWindow.Height += RowHeight;
            Application.Current.MainWindow.MinHeight += RowHeight;
            Grid.SetRowSpan(Vertical_Rectangle, Grid.GetRowSpan(Vertical_Rectangle) + (withSeparator ? 2 : 1));
            Grid.SetRowSpan(Preview_Image, Grid.GetRowSpan(Preview_Image) + (withSeparator ? 2 : 1));
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
                IsEnabled = File.Exists(button.File),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            control.Checked += new RoutedEventHandler((sender, e) =>
            {
                UpdatePreviewControls(name);
            });

            Grid.SetRow(control, row);
            Grid.SetColumn(control, 5);

            MainGrid.Children.Add(control);
            PreviewRadioButtons.Add(control);

            if (isChecked)
            {
                UpdatePreviewControls(name);
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
                ClickRadioButton(Preview_RadioButton_ChoosenFile);
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
            var generateImages = GenerateImages;
            ThreadManager.RunActionInNewThread(async () => await mediator.Send(new CardGeneratingCommand(fileName, generateImages, cts), cts.Token));
        }

        private void PreparePdf(string fileName)
        {
            var cts = new CancellationTokenSource();
            var generateImages = GenerateImages;
            ThreadManager.RunActionInNewThread(async () => await mediator.Send(new PdfGeneratingCommand(fileName, generateImages, cts), cts.Token));
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
        }

        private void NextPreview_Button_Click(object sender, RoutedEventArgs e)
        {
            Preview_Image.Source = previewFactory.NextPreviewImage(GridWidth, GridHeight).GetAwaiter().GetResult();
        }

        private void Preview_RadioButton_ChoosenFile_Click(object sender, RoutedEventArgs e)
        {
            UpdatePreviewControls(ChoosenFile);
        }

        private void GenerateImages_Checkbox_Click(object sender, RoutedEventArgs e) =>
            RefreshPreview();

        private void Grid_TextBox_LostFocus(object sender, RoutedEventArgs e) =>
            Preview_Image.Source = previewFactory.GetPreviewImage(GridWidth, GridHeight).GetAwaiter().GetResult();

        private void RefreshPreview()
        {
            previewFactory.Refresh(GenerateImages).GetAwaiter().GetResult();
            Preview_Image.Source = previewFactory.GetPreviewImage(GridWidth, GridHeight).GetAwaiter().GetResult();
        }

        private void PreviewAutoRefresh_Checkbox_Click(object sender, RoutedEventArgs e)
        {
            UpdatePreviewTimer();
        }

        private void UpdatePreviewTimer(bool? isEnabled = null)
        {
            if (isEnabled ?? PreviewAutoRefresh_Checkbox.IsChecked == true)
            {
                if (!PreviewTimer.IsEnabled)
                    PreviewTimer.Start();
            }
            else if (PreviewTimer.IsEnabled)
                PreviewTimer.Stop();
        }

        private void UpdatePreviewControls(string key)
        {
            if (key != null)
            {
                previewFactory.SetCurrentPreview(key, GenerateImages).GetAwaiter().GetResult();
                Preview_Image.Source = previewFactory.GetPreviewImage(GridWidth, GridHeight).GetAwaiter().GetResult();
            }
            else
            {
                Preview_Image.Source = null;
            }
            Preview_RadioButton_ChoosenFile.IsEnabled = !string.IsNullOrEmpty(ChooseFileDialog.FileName);

            var visibility = Preview_Image.Source == null ? Visibility.Hidden : Visibility.Visible;
            PreviewAutoRefresh_Checkbox.Visibility = visibility;
            PreviousPreview_Button.Visibility = visibility;
            NextPreview_Button.Visibility = visibility;
            GridHeight_TextBox.Visibility = visibility;
            GridHeight_Label.Visibility = visibility;
            GridWidth_TextBox.Visibility = visibility;
            GridWidth_Label.Visibility = visibility;
            PreviewHidden_Label.Visibility = Preview_Image.Source == null ? Visibility.Visible : Visibility.Hidden;

            if (key == null)
                UpdatePreviewTimer(false);
            else
                UpdatePreviewTimer();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    if (PreviousPreview_Button.IsEnabled && PreviousPreview_Button.IsVisible)
                        ClickButton(PreviousPreview_Button);
                    e.Handled = true;
                    return;
                case Key.Right:
                    if (NextPreview_Button.IsEnabled && NextPreview_Button.IsVisible)
                        ClickButton(NextPreview_Button);
                    e.Handled = true;
                    return;
                case Key.Up:
                    ClickPreviousPreviewRadioButton();
                    e.Handled = true;
                    return;
                case Key.Down:
                    ClickNextPreviewRadioButton();
                    e.Handled = true;
                    return;
                default:
                    return;
            }
        }

        private void ClickPreviousPreviewRadioButton()
        {
            var choosenIdx = PreviewRadioButtons.FindIndex(button => button.IsChecked == true);
            for (var i = choosenIdx - 1; i >= 0; i--)
            {
                var button = PreviewRadioButtons[i];
                if (button.IsEnabled && button.IsVisible)
                {
                    ClickRadioButton(button);
                    return;
                }
            }
        }

        private void ClickNextPreviewRadioButton()
        {
            var choosenIdx = PreviewRadioButtons.FindIndex(button => button.IsChecked == true);
            for (var i = choosenIdx + 1; i < PreviewRadioButtons.Count; i++)
            {
                var button = PreviewRadioButtons[i];
                if (button.IsEnabled && button.IsVisible)
                {
                    ClickRadioButton(button);
                    return;
                }
            }
        }

        private void ClickButton(Button button)
        {
            var peer = new ButtonAutomationPeer(button);
            var invokeProvider = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
            invokeProvider.Invoke();
        }

        private void ClickRadioButton(RadioButton button)
        {
            var peer = new RadioButtonAutomationPeer(button);
            var selectionItemProvider = peer.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider;
            selectionItemProvider.Select();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            Environment.Exit(0);
        }
    }
}

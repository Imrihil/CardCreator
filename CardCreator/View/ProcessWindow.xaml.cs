using CardCreator.Extensions;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace CardCreator.View
{
    /// <summary>
    /// Interaction logic for ProcessWindow.xaml
    /// </summary>
    public partial class ProcessWindow : Window
    {
        private CancellationTokenSource Cts { get; set; }

        public ProcessWindow()
        {
            InitializeComponent();
            Ok_Button.IsEnabled = false;
            Cancel_Button.IsEnabled = false;
        }

        public void RegisterCancelationToken(CancellationTokenSource cts)
        {
            Cts = cts;
            Cancel_Button.IsEnabled = true;
        }

        private void Ok_Button_Click(object sender, RoutedEventArgs e)
        {
            Cts = null;
            Close();
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            Cts?.Cancel();
            Close();
        }

        public void UpdateProgressBar(double value, Visibility visibility = Visibility.Visible)
        {
            ProgressBar.Visibility = visibility;
            ProgressBarText.Visibility = visibility;
            ProgressBar.Value = value;

            if (value >= 100)
                Ok_Button.IsEnabled = true;

            ProgressBar.Refresh();
            ProgressBarText.Refresh();
        }

        public void UpdateTextBlockResultMessage(string text, Visibility visibility = Visibility.Visible)
        {
            TextBoxResultMessage.Visibility = visibility;
            TextBoxResultMessage.Text = text;

            TextBoxResultMessage.Refresh();
        }

        public void AppendTextBlockResultMessage(string text, Visibility visibility = Visibility.Visible)
        {
            TextBoxResultMessage.Visibility = visibility;
            if (string.IsNullOrEmpty(text)) return;

            TextBoxResultMessage.Text = (string.IsNullOrEmpty(TextBoxResultMessage.Text) ? "" : TextBoxResultMessage.Text + "\n") + text;
            TextBoxResultMessage.ScrollToEnd();

            TextBoxResultMessage.Refresh();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Cts?.Cancel();
            base.OnClosing(e);
        }
    }
}

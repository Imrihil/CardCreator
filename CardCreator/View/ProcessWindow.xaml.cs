using CardCreator.Extensions;
using CardCreator.Features.Logging;
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
    public partial class ProcessWindow : Window, ILogger
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

        public void SetProgress(double value)
        {
            ProgressBar.Value = value;

            if (value >= 100)
                Ok_Button.IsEnabled = true;

            ProgressBar.Refresh();
            ProgressBarText.Refresh();
        }

        public void LogMessage(object message)
        {
            TextBoxResultMessage.Text = (string.IsNullOrEmpty(TextBoxResultMessage.Text) ? "" : TextBoxResultMessage.Text + "\n") + message;
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

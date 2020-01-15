using CardCreator.Extensions;
using CardCreator.Features.Logging;
using CardCreator.Features.Threading;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

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
            ThreadManager.RunActionWithDispatcher(Dispatcher, () => Cancel_Button.IsEnabled = true);
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
            ThreadManager.RunActionWithDispatcher(Dispatcher, () =>
            {
                ProgressBar.Value = value;

                if (value >= 100)
                    Ok_Button.IsEnabled = true;

                ProgressBar.Refresh();
                ProgressBarText.Refresh();
            });
        }

        public void LogMessage(object message)
        {
            ThreadManager.RunActionWithDispatcher(Dispatcher, () =>
            {
                TextBoxResultMessage.Text = (string.IsNullOrEmpty(TextBoxResultMessage.Text) ? "" : TextBoxResultMessage.Text + "\n") + message;
                TextBoxResultMessage.ScrollToEnd();

                TextBoxResultMessage.Refresh();
            });
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            Cts?.Cancel();
            Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Background);
        }
    }
}

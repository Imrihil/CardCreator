using MyWarCreator.Extensions;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MyWarCreator.Processing
{
    /// <summary>
    /// Interaction logic for ProcessWindow.xaml
    /// </summary>
    public partial class ProcessWindow : Window
    {
        public ProcessWindow()
        {
            InitializeComponent();
            ProgressBar.Visibility = Visibility.Collapsed;
            ProgressBarText.Visibility = Visibility.Collapsed;
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        { }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        { }

        private long progressBarLastRefresh;
        private void UpdateProgressBar(double value, Visibility visibility = Visibility.Visible)
        {
            ProgressBar.Visibility = visibility;
            ProgressBarText.Visibility = visibility;
            ProgressBar.Value = value;
            var time = DateTime.Now.Ticks;
            if (time - progressBarLastRefresh <= 1000) return;

            ProgressBar.Refresh();
            ProgressBarText.Refresh();
            progressBarLastRefresh = time;
        }

        private long textBoxResultMessageLastRefresh;
        private void UpdateTextBlockResultMessage(string text, Visibility visibility = Visibility.Visible)
        {
            TextBoxResultMessage.Visibility = visibility;
            TextBoxResultMessage.Text = text;
            var time = DateTime.Now.Ticks;
            if (time - textBoxResultMessageLastRefresh <= 1000) return;

            TextBoxResultMessage.Refresh();
            textBoxResultMessageLastRefresh = time;
        }
        private void AppendTextBlockResultMessage(string text, Visibility visibility = Visibility.Visible)
        {
            TextBoxResultMessage.Visibility = visibility;
            if (string.IsNullOrEmpty(text)) return;

            TextBoxResultMessage.Text = (string.IsNullOrEmpty(TextBoxResultMessage.Text) ? "" : TextBoxResultMessage.Text + "\n") + text;
            TextBoxResultMessage.ScrollToEnd();
            var time = DateTime.Now.Ticks;
            if (time - textBoxResultMessageLastRefresh <= 1000) return;

            TextBoxResultMessage.Refresh();
            textBoxResultMessageLastRefresh = time;
        }
    }
}

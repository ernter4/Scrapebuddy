using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ScrapeBuddy.ViewModels
{
    /// <summary>
    /// Represents a dialog window that displays a loading indicator and message during scraping operations (also shows error or content of scrape).
    /// </summary>
    public partial class ScrapeDialog : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScrapingDialog"/> class.
        /// </summary>
        public ScrapeDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Updates the displayed message in the loading dialog.
        /// </summary>
        /// <param name="message">The message to display.</param>
        public void UpdateMessage(string message)
        {
            MessageTextBlock.Text = message;
        }

        /// <summary>
        /// Shows the OK button and hides the loading progress bar.
        /// </summary>
        public void ShowOkButton()
        {
            LoadingProgressBar.IsVisible = false; // Hide the loading indicator
            OkButton.IsVisible = true; // Show the OK button
        }

        /// <summary>
        /// Handles the OK button click event to close the dialog.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
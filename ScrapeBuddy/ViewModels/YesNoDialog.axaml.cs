using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ScrapeBuddy.ViewModels
{
    /// <summary>
    /// Window for confirmation to reset the passphrase.
    /// </summary>
    public partial class YesNoDialog : Window
    {
        /// <summary>
        /// Initializes the window and button handlers.
        /// </summary>
        public YesNoDialog()
        {
            InitializeComponent();
            this.FindControl<Button>("YesButton").Click += YesButton_Click;
            this.FindControl<Button>("NoButton").Click += NoButton_Click;
        }

        /// <summary>
        /// Loads the XAML content.
        /// </summary>
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void UpdateConfirmationMessage(string newMessage)
        {
            var textBlock = this.FindControl<TextBlock>("ConfirmationMessageTextBlock");
            if (textBlock != null)
            {
                textBlock.Text = newMessage;
            }
            else
            {
                // Log or handle the case where the TextBlock was not found
                System.Diagnostics.Debug.WriteLine("ConfirmationMessageTextBlock not found.");
            }
        }

        /// <summary>
        /// Handles "Yes" button click, closes dialog with true.
        /// </summary>
        private void YesButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close(true);
        }

        /// <summary>
        /// Handles "No" button click, closes dialog with false.
        /// </summary>
        private void NoButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close(false);
        }
    }
}
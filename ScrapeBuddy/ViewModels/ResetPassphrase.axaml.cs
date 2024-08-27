using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ScrapeBuddy.ViewModels
{
    /// <summary>
    /// Window for confirmation to reset the passphrase.
    /// </summary>
    public partial class ResetPassphraseWindow : Window
    {
        /// <summary>
        /// Initializes the window and button handlers.
        /// </summary>
        public ResetPassphraseWindow()
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
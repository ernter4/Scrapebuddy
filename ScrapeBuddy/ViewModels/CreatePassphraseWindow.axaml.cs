using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using ScrapeBuddy.Logic;
using ScrapeBuddy.Views;

namespace ScrapeBuddy
{
    /// <summary>
    /// Represents the window for creating a new passphrase.
    /// </summary>
    public partial class CreatePassphraseWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatePassphraseWindow"/> class.
        /// </summary>
        public CreatePassphraseWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the click event of the "Create New Password" button.
        /// Validates the password and, if valid, hashes it and proceeds to the main application window.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private async void OnCreateNewPasswordClick(object? sender, RoutedEventArgs e)
        {
            string password = PasswordBox.Text;
            string passwordConfirm = ConfirmPasswordBox.Text;

            if (password == passwordConfirm)
            {
                // Validate password
                var errors = PasswordValidator.ValidatePassword(password);
                if (errors.All(passwordValid => passwordValid))
                {
                    Hashing hash = new Hashing(password);
                    await hash.GenerateHashAsync();

                    var splashScreen = new LoadingWindow();
                    splashScreen.Show();
                    Close();

                    await splashScreen.InitApp();

                    var main = new MainWindow
                    {
                        DataContext = new MainWindow(),
                    };

                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                    Close();

                    splashScreen.Close();
                }
            }
            else
            {
                ErrorTextBlock.Text = "Passwords do not match. Please try again.";
                ErrorTextBlock.IsVisible = true;
            }
        }

        /// <summary>
        /// Handles the text changed event for text boxes to hide the error message.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void Box_TextChanged(object? sender, Avalonia.Controls.TextChangedEventArgs e)
        {
            ErrorTextBlock.IsVisible = false;
        }

        /// <summary>
        /// Handles the text changed event for the password box. Updates the password criteria indicators.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void PasswordBox_TextChanged(object? sender, Avalonia.Controls.TextChangedEventArgs e)
        {
            string password = PasswordBox.Text;
            ErrorTextBlock.IsVisible = false;
            
            List<bool> passwordValid = PasswordValidator.ValidatePassword(password);
            
            var criteria = new[]
            {
                new { TextBlock = LengthCriteria, IsValid = passwordValid[0] },
                new { TextBlock = UppercaseCriteria, IsValid = passwordValid[1] },
                new { TextBlock = LowercaseCriteria, IsValid = passwordValid[2] },
                new { TextBlock = DigitCriteria, IsValid = passwordValid[3] },
                new { TextBlock = SpecialCharCriteria, IsValid = passwordValid[4] }
            };
            
            foreach (var criterion in criteria)
            {
                criterion.TextBlock.Text = criterion.IsValid ? "✔ " + criterion.TextBlock.Text.Substring(2) : "✘ " + criterion.TextBlock.Text.Substring(2);
                criterion.TextBlock.Foreground = criterion.IsValid ? Brushes.Green : Brushes.Red;
            }
        }
    }
}


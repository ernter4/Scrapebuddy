using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using ScrapeBuddy.Logic;
using ScrapeBuddy.Data;
using ScrapeBuddy.ViewModels;

namespace ScrapeBuddy.ViewModels
{
    /// <summary>
    /// Represents the window for creating a new passphrase.
    /// </summary>
    public partial class CreatePassphraseWindow : Window
    {
        private NotificationViewModel NotificationsView { get; set; }
        private bool reset = false;
        private List<DecryptedUser>? users;
        private (string email, string name)? emailAndName;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatePassphraseWindow"/> class.
        /// </summary>
        /// <param name="reset">If the user wants to reset the passphrase</param>
        /// <param name="users">The userdata which needs to be encrypted with the new passphrase.</param>
        /// <param name="emailAndName">The user email and name which needs to be encrypted with the new passphrase.</param>
        public CreatePassphraseWindow(bool reset, List<DecryptedUser>?  users, (string email, string name)? emailAndName)
        {
            InitializeComponent();
            this.users = users;
            this.reset = reset;
            this.emailAndName = emailAndName;
            NotificationsView = new NotificationViewModel();
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
                var errors = PasswordValidator.ValidatePassword(password);
                if (errors.All(passwordValid => passwordValid))
                {
                    Hashing hash = new Hashing(password);
                    await hash.GenerateHashAsync();
                    hash.StoreMasterKey();
                    
                    if (reset)
                    {
                        Hashing.DeleteHash();
                        DatabaseHandler _databaseHandler = new DatabaseHandler();
                        _databaseHandler.ReEncryptUserData(users);
                        if (emailAndName != null)
                        { 
                            var (email, name) = emailAndName.Value;
                            Encryption enc = new Encryption(email, name);
                            var resultEnc = enc.EncryptStringToBytes_Aes();
                            byte[] Email = resultEnc.encUsername;
                            byte[] Name = resultEnc.encPassword;
                            byte[] IV = resultEnc.IV;
                            byte[] Salt = resultEnc.salt;
                            
                            var settingsManager = new SettingsManager();
                            settingsManager.UpdateSettings(Email, Name, Salt, IV);
                        }
                    }
                    
                    var splashScreen = new LoadingWindow();
                    splashScreen.Show();
                    Close();

                    await splashScreen.InitApp();

                    var mainWindow = new MainWindow
                    {
                        DataContext = new MainWindow(),
                    };
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


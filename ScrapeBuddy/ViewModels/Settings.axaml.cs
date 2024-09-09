using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Google.Protobuf;
using ScrapeBuddy.Data;
using ScrapeBuddy.Logic;

namespace ScrapeBuddy.ViewModels
{
    /// <summary>
    /// ViewModel for managing settings in the application.
    /// </summary>
    public partial class SettingsViewModel : UserControl
    {
        private SettingsManager _settingsManager;
        private SettingsModel _settings;
        private bool _isEmailNotificationsEnabled;
        private string? _email;
        private string? _name;

        /// <summary>
        /// Event triggered when the cancel action is requested.
        /// </summary>
        public event EventHandler CancelRequested;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
        /// </summary>
        public SettingsViewModel()
        {
            InitializeComponent();
            _settingsManager = new SettingsManager();
            _settings = _settingsManager.LoadSettings();
            LoadSettings();
            EmailTextBox.TextChanged += OnEmailTextBoxTextChanged;
            NameTextBox.TextChanged += OnNameTextBoxTextChanged;
            UpdateSaveButtonState();
        }

        /// <summary>
        /// Loads settings and applies them to the view.
        /// </summary>
        public void LoadSettings()
        {
            DarkModeToggleSwitch.IsChecked = _settings?.DarkMode ?? false;
            EmailNotificationsToggleSwitch.IsChecked = _settings?.EmailNotificationsEnabled ?? false;
            if (_settings.EmailNotificationsEnabled)
            {
                Decryption dec = new Decryption(_settings.Email, _settings.Name, _settings.Salt, _settings.IV);
                var resultDec = dec.DecryptStringFromBytes_Aes();
                _email = resultDec.username;
                _name = resultDec.password;
            }
            else
            {
                _email = null;
                _name = null;
            }

            EmailTextBox.Text = _email ?? string.Empty;
            NameTextBox.Text = _name ?? string.Empty;
            EmailSettingsPanel.IsVisible = _settings?.EmailNotificationsEnabled ?? false;
        }

        /// <summary>
        /// Saves the dark mode setting.
        /// </summary>
        private void SaveDarkModeSetting()
        {
            _settings.DarkMode = DarkModeToggleSwitch.IsChecked ?? false;
            _settingsManager.SaveSettings(_settings);
        }

        /// <summary>
        /// Handles the cancel button click event.
        /// </summary>
        private void OnCancelView(object sender, RoutedEventArgs args)
        {
            CancelRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handles the toggle event for dark and white mode.
        /// </summary>
        private void OnToggleDarkWhiteMode(object sender, RoutedEventArgs args)
        {
            SaveDarkModeSetting();
        }

        /// <summary>
        /// Handles the toggle event for email notifications.
        /// </summary>
        private void OnToggleEmailNotifications(object sender, RoutedEventArgs args)
        {
            _isEmailNotificationsEnabled = EmailNotificationsToggleSwitch.IsChecked ?? false;
            EmailSettingsPanel.IsVisible = _isEmailNotificationsEnabled;

            if (!_isEmailNotificationsEnabled)
            {
                EmailTextBox.Text = string.Empty;
                NameTextBox.Text = string.Empty;
                _email = null;
                _name = null;
                _settings.EmailNotificationsEnabled = false;
                _settings.Email = null;
                _settings.Name = null;
                _settings.Salt = null;
                _settings.IV = null;
                _settingsManager.SaveSettings(_settings);
            }

            UpdateSaveButtonState();
        }

        /// <summary>
        /// Handles the save button click event for email settings.
        /// </summary>
        private void OnSaveEmailSettings(object sender, RoutedEventArgs args)
        {
            _settings.EmailNotificationsEnabled = true;
            _email = EmailTextBox.Text;
            _name = NameTextBox.Text;
            Encryption enc = new Encryption(_email, _name);
            var resultEnc = enc.EncryptStringToBytes_Aes();

            _settings.Email = resultEnc.encUsername;
            _settings.Name = resultEnc.encPassword;
            _settings.IV = resultEnc.IV;
            _settings.Salt = resultEnc.salt;
            _settingsManager.SaveSettings(_settings);
        }

        /// <summary>
        /// Handles the click event for deleting the database.
        /// </summary>
        private async void OnDeleteDatabase(object sender, RoutedEventArgs args)
        {
            var parentWindow = this.GetVisualRoot() as Window;
            var confirmationDialog = new YesNoDialog();
            confirmationDialog.UpdateConfirmationMessage("This will permanently delete your data. Do you really want that?");
            bool result = await confirmationDialog.ShowDialog<bool>(parentWindow);

            if (result)
            {
                var initialWindow = new CreatePassphraseWindow(false, null, null);
                initialWindow.Show();
                parentWindow?.Close();
                DatabaseHandler.DeleteAllTables();
                Hashing.DeleteHash();
                _settingsManager.DeleteSettings();
            }
        }

        /// <summary>
        /// Handles the click event for resetting the passphrase.
        /// </summary>
        private async void OnResetPassphrase(object sender, RoutedEventArgs args)
        {
            var parentWindow = this.GetVisualRoot() as Window;
            var confirmationDialog = new YesNoDialog();
            confirmationDialog.UpdateConfirmationMessage("Do you really want to change your passphrase?");
            bool result = await confirmationDialog.ShowDialog<bool>(parentWindow);
            
            if (result)
            {
               DatabaseHandler _databaseHandler = new DatabaseHandler();
               var decryptedUsers = _databaseHandler.DecryptAllUserData();
               (string email, string name)? emailAndName = null;
               if (_settings.EmailNotificationsEnabled)
               {
                   Decryption dec = new Decryption(_settings.Email, _settings.Name, _settings.Salt, _settings.IV);
                   var resultDec = dec.DecryptStringFromBytes_Aes();
                   _email = resultDec.username;
                   _name = resultDec.password;
                   emailAndName = (resultDec.username, resultDec.password);
               }
               var passphraseWindow = new CreatePassphraseWindow(true, decryptedUsers, emailAndName);
               passphraseWindow.Show();
               parentWindow?.Close();
            }
        }

        /// <summary>
        /// Handles the text changed event for the email text box.
        /// </summary>
        private void OnEmailTextBoxTextChanged(object sender, Avalonia.Controls.TextChangedEventArgs e)
        {
            UpdateSaveButtonState();
        }

        /// <summary>
        /// Handles the text changed event for the name text box.
        /// </summary>
        private void OnNameTextBoxTextChanged(object sender, Avalonia.Controls.TextChangedEventArgs e)
        {
            UpdateSaveButtonState();
        }

        /// <summary>
        /// Updates the state of the save button based on the current settings.
        /// </summary>
        private void UpdateSaveButtonState()
        {
            bool isSaveEnabled = _isEmailNotificationsEnabled 
                                 && !string.IsNullOrWhiteSpace(EmailTextBox.Text) 
                                 && !string.IsNullOrWhiteSpace(NameTextBox.Text);

            SaveButton.IsEnabled = isSaveEnabled;
        }
    }
}


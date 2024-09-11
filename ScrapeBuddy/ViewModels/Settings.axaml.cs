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
using Newtonsoft.Json.Linq;
using System.IO;
using Avalonia;
using Avalonia.Media;

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
        private bool Editable;
        public static event EventHandler ThemeChanged;
        public event EventHandler CancelRequested;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
        /// </summary>
        public SettingsViewModel()
        {
            InitializeComponent();
            ThemeChanged += SetTheme;
            SetTheme(null, null);
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

            Editable = _settings?.EmailNotificationsEnabled ?? false;
            
            if (Editable = false)
            {
                SaveButton.Content = "Save";
                EmailTextBox.IsEnabled = true;
                NameTextBox.IsEnabled = true;
            }
            else
            {
                SaveButton.Content = "Edit";
                EmailTextBox.IsEnabled = false;
                NameTextBox.IsEnabled = false;
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
            ThemeChanged?.Invoke(null, EventArgs.Empty);
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
            else
            {
                
                Editable = true;
                SaveButton.Content = "Save";
                EmailTextBox.IsEnabled = true;
                NameTextBox.IsEnabled = true;
            }
            
            UpdateSaveButtonState();
        }

        /// <summary>
        /// Handles the save button click event for email settings.
        /// </summary>
        private void OnSaveEmailSettings(object sender, RoutedEventArgs args)
        {
            if (Editable)
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
                SaveButton.Content = "Edit";
                EmailTextBox.IsEnabled = false;
                NameTextBox.IsEnabled = false;
            } else
            {
                SaveButton.Content = "Save";
                EmailTextBox.IsEnabled = true;
                NameTextBox.IsEnabled = true;
            }
            
            Editable = !Editable;
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
                _settingsManager.WriteDefaultValues();
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
            bool isSaveEnabled = true;
            if (Editable)
            {
                isSaveEnabled =  !string.IsNullOrWhiteSpace(EmailTextBox.Text)
                                     && !string.IsNullOrWhiteSpace(NameTextBox.Text);
            }

            SaveButton.IsEnabled = isSaveEnabled;
        }
        
        /// <summary>
        /// Sets the application theme based on the "DarkMode" setting.
        /// </summary>
        /// <param name="sender">The event sender (can be null).</param>
        /// <param name="eventArgs">Event arguments (can be null).</param>
        private void SetTheme(object? sender, EventArgs eventArgs)
        {
            JObject settingsJson = JObject.Parse(File.ReadAllText(@"Data/Settings.json"));
            bool isDarkMode = settingsJson["DarkMode"].ToObject<bool>();
            if (isDarkMode)
            {
                Console.WriteLine("Now its Darkmode");
                MainGrid.Background = (SolidColorBrush)Application.Current.Resources["BackgroundDark"];
                Home.Classes.Clear();
                Home.Classes.Add("btn_white");
                SaveButton.Classes.Clear();
                SaveButton.Classes.Add("btn_white");
                DeleteDatabase.Classes.Clear();
                DeleteDatabase.Classes.Add("btn_white");
                ResetPassphrase.Classes.Clear();
                ResetPassphrase.Classes.Add("btn_white");
                Title.Foreground = new SolidColorBrush(Color.FromRgb(239, 239, 239));
                DarkModeToggleSwitch.Foreground = new SolidColorBrush(Color.FromRgb(239, 239, 239));
                EmailNotificationsToggleSwitch.Foreground = new SolidColorBrush(Color.FromRgb(239, 239, 239));
            }
            else
            {
                Console.WriteLine("Now its Lightmode");
                MainGrid.Background = (SolidColorBrush)Application.Current.Resources["BackgroundLight"];
                Home.Classes.Clear();
                Home.Classes.Add("btn_dark");
                SaveButton.Classes.Clear();
                SaveButton.Classes.Add("btn_dark");
                DeleteDatabase.Classes.Clear();
                DeleteDatabase.Classes.Add("btn_dark");
                ResetPassphrase.Classes.Clear();
                ResetPassphrase.Classes.Add("btn_dark");
                Title.Foreground = new SolidColorBrush(Color.FromRgb(40, 40, 40));
                DarkModeToggleSwitch.Foreground = new SolidColorBrush(Color.FromRgb(40, 40, 40));
                EmailNotificationsToggleSwitch.Foreground = new SolidColorBrush(Color.FromRgb(40, 40, 40));
            }
        }
    }
}


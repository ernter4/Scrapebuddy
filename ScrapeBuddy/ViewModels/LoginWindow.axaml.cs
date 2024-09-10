using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using ScrapeBuddy.Data;
using ScrapeBuddy.ViewModels;
using ScrapeBuddy.Logic;
using ScrapeBuddy.Exceptions;
using Newtonsoft.Json.Linq;
using System.IO;

namespace ScrapeBuddy.ViewModels
{
    
    
    /// <summary>
    /// Represents the login window where users can enter their credentials.
    /// </summary>
    public partial class LoginWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginWindow"/> class.
        /// </summary>
        public LoginWindow()
        {
            InitializeComponent(); SettingsViewModel.ThemeChanged += SetTheme;
            SetTheme(null, null);
            
        }
        

        /// <summary>
        /// Handles the click event for the login button.
        /// Verifies the user's password, stores the master key, triggers the ScrapeItem Method and initializes the application.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private async void OnLoginClick(object? sender, RoutedEventArgs e)
        {
            Hashing hash = new Hashing(PasswordBox.Text);

            if (hash.Verify())
            {
                hash.StoreMasterKey();
                
                var splashScreen = new LoadingWindow();
                splashScreen.Show();
                Close();

                var scrapeTask = Task.Run(() => Scrape.ScrapeItems());

                await splashScreen.InitApp();

                var mainWindow = new MainWindow
                {
                    DataContext = new MainWindow(),
                };
                mainWindow.Show();

                splashScreen.Close();

                await scrapeTask;
            }
            else
            {
                ErrorTextBlock.Text = "Incorrect password. Please try again.";
                ErrorTextBlock.IsVisible = true;
            }
        }
        
        /// <summary>
        /// Hides the error message when the text in the PasswordBox changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data about the text change.</param>
        private void PasswordBox_TextChanged(object? sender, Avalonia.Controls.TextChangedEventArgs e)
        {
            ErrorTextBlock.IsVisible = false;
        }

        /// <summary>
        /// Handles the "Forgot Password" button click. Shows a dialog to reset the password.
        /// If confirmed, clears the database and opens a new passphrase window.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data for the button click.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async void ForgotPasswordClick(object? sender, RoutedEventArgs e)
        {
            var confirmationDialog = new YesNoDialog();
            bool result = await confirmationDialog.ShowDialog<bool>(this);

            if (result)
            {
                DatabaseHandler.DeleteAllTables();
                Hashing.DeleteHash();

                var initialWindow = new CreatePassphraseWindow(false, null, null);
                initialWindow.Show();
        
                Close();
            }
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
            }
            else
            {
                Console.WriteLine("Now its Lightmode");
            }
        }
    }
}

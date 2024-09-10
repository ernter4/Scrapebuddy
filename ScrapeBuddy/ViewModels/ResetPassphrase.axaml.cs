using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Newtonsoft.Json.Linq;
using System.IO;

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
            SettingsViewModel.ThemeChanged += SetTheme;
            SetTheme(null, null);
            
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
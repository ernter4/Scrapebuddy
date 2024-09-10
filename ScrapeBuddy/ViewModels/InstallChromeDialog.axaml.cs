using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net.Mime;
using Avalonia.Media;
using Avalonia; 

namespace ScrapeBuddy.ViewModels
{
    /// <summary>
    /// Window for confirmation to reset the passphrase.
    /// </summary>
    public partial class InstallChromeDialog : Window
    {
        /// <summary>
        /// Initializes the window and button handlers.
        /// </summary>
        public InstallChromeDialog()
        {
            InitializeComponent();
            MainGrid = this.FindControl<Grid>("MainGrid");
            OkayButton = this.FindControl<Button>("OkayButton");
            Text = this.FindControl<TextBlock>("Text");

            if (MainGrid == null || OkayButton == null | Text == null)
            {
                Console.WriteLine("Failed to find controls.");
                return;
            }

            OkayButton.Click += OkayButton_Click;
            SettingsViewModel.ThemeChanged += SetTheme;
            
            this.Loaded += (sender, e) =>
            {
                SetTheme(null, null);
            };
        }
        
        /// <summary>
        /// Loads the XAML content.
        /// </summary>
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        
        /// <summary>
        /// Handles "Okay" button click, closes dialog with.
        /// </summary>
        private void OkayButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close();
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
                OkayButton.Classes.Clear();
                OkayButton.Classes.Add("btn_white");
                Text.Foreground = new SolidColorBrush(Colors.White);
            }
            else
            {
                Console.WriteLine("Now its Lightmode");
                MainGrid.Background = (SolidColorBrush)Application.Current.Resources["BackgroundLight"];
                OkayButton.Classes.Clear();
                OkayButton.Classes.Add("btn_dark");
                Text.Foreground = new SolidColorBrush(Colors.Black);
            }
        }
    }
}
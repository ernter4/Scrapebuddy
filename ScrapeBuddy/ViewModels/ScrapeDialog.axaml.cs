using Avalonia.Controls;
using Avalonia.Interactivity;
using Newtonsoft.Json.Linq;
using System.IO;
using System;
using Avalonia;
using Avalonia.Controls.Documents;
using Avalonia.Media;

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
            MainGrid = this.FindControl<Grid>("MainGrid");
            OkayButton = this.FindControl<Button>("OkayButton");
            Text = this.FindControl<TextBlock>("Text");

            if (MainGrid == null || OkayButton == null | Text == null)
            {
                Console.WriteLine("Failed to find controls.");
                return;
            }
            
            SettingsViewModel.ThemeChanged += SetTheme;
            
            this.Loaded += (sender, e) =>
            {
                SetTheme(null, null);
            };
        }

        /// <summary>
        /// Updates the displayed message in the loading dialog.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="isSuccess">Indicates if the scraping was successful</param>
        public void UpdateMessage(string message, bool isSuccess)
        {
            Text.Text = "";

            Run symbolRun = new Run();
            if (isSuccess)
            {
                symbolRun.Text = "✔️ "; 
                symbolRun.Foreground = new SolidColorBrush(Colors.Green);  
            }
            else
            {
                symbolRun.Text = "❌ ";  
                symbolRun.Foreground = new SolidColorBrush(Colors.Red);  
            }
            
            Run messageRun = new Run(message);
            
            Text.Inlines.Add(symbolRun);  
            Text.Inlines.Add(messageRun);  
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
                Text.Foreground = new SolidColorBrush(Color.FromRgb(239, 239, 239));
            }
            else
            {
                Console.WriteLine("Now its Lightmode");
                MainGrid.Background = (SolidColorBrush)Application.Current.Resources["BackgroundLight"];
                OkayButton.Classes.Clear();
                OkayButton.Classes.Add("btn_dark");
                Text.Foreground = new SolidColorBrush(Color.FromRgb(40, 40, 40));
            }
        }
    }
}
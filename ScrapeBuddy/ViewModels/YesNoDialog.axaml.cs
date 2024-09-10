using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using Newtonsoft.Json.Linq;
using System.IO;
using Avalonia.Media;
using Avalonia;

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
            
            MainGrid = this.FindControl<Grid>("MainGrid");
            YesButton= this.FindControl<Button>("YesButton");
            NoButton= this.FindControl<Button>("NoButton");
            ConfirmationMessageTextBlock = this.FindControl<TextBlock>("ConfirmationMessageTextBlock");

            if (MainGrid == null || YesButton == null || NoButton == null || ConfirmationMessageTextBlock == null)
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
                YesButton.Classes.Clear();
                YesButton.Classes.Add("btn_white"); 
                NoButton.Classes.Clear();
                NoButton.Classes.Add("btn_white"); 
                ConfirmationMessageTextBlock.Foreground = new SolidColorBrush(Colors.White);
            }
            else
            {
                Console.WriteLine("Now its Lightmode");
                MainGrid.Background = (SolidColorBrush)Application.Current.Resources["BackgroundLight"];
                YesButton.Classes.Clear();
                YesButton.Classes.Add("btn_dark"); 
                NoButton.Classes.Clear();
                NoButton.Classes.Add("btn_dark"); 
                ConfirmationMessageTextBlock.Foreground = new SolidColorBrush(Colors.Black);
            }
        }
    }
}
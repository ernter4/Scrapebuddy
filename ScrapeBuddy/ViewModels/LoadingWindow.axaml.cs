using Avalonia.Controls;
using Avalonia.Threading;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json.Linq;
using System.IO;
using Avalonia;
using Avalonia.Media;

namespace ScrapeBuddy.ViewModels
{
    /// <summary>
    /// Represents a window that displays a loading screen with a progress bar.
    /// </summary>
    public partial class LoadingWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoadingWindow"/> class.
        /// </summary>
        public LoadingWindow()
        {
            InitializeComponent();SettingsViewModel.ThemeChanged += SetTheme;
            SetTheme(null, null);
            
        }

        /// <summary>
        /// Simulates the initialization of the application, updating the progress bar and status text.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task InitApp()
        {
            var start = DateTime.Now.Ticks;
            var time = start;
            var progressValue = 0;

            // Increment progress value over the course of one second
            while ((time - start) < TimeSpan.TicksPerSecond)
            {
                progressValue++;
                Dispatcher.UIThread.Post(() => ProgressBar1.Value = progressValue);
                await Task.Delay(25);
                time = DateTime.Now.Ticks;
            }

            start = time;
            Dispatcher.UIThread.Post(() => LoadingText.Text = "Initializing Application Settings...");

            // Increment progress value over the course of two seconds while displaying status
            var limit = TimeSpan.TicksPerSecond * 2;
            while ((time - start) < limit)
            {
                progressValue += 1;
                Dispatcher.UIThread.Post(() => ProgressBar1.Value = progressValue);
                await Task.Delay(50);
                time = DateTime.Now.Ticks;
            }

            Dispatcher.UIThread.Post(() => LoadingText.Text = "Preparing App...");

            // Increment progress to 100% in small steps
            while (progressValue < 100)
            {
                progressValue += 1;
                Dispatcher.UIThread.Post(() => ProgressBar1.Value = progressValue);
                await Task.Delay(10);
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
                Console.WriteLine("Now its Darkmode");
                Background = (SolidColorBrush)Application.Current.Resources["BackgroundDark"];
                Block.Foreground = new SolidColorBrush(Color.Parse("#81D2C7"));
                
            }
            else
            {
                Console.WriteLine("Now its Lightmode"); 
                Console.WriteLine("Now its Lightmode"); 
                Background = (SolidColorBrush)Application.Current.Resources["BackgroundLight"]; 
                Block.Foreground = new SolidColorBrush(Color.Parse("#416788"));
                
            }
        }
    }
}

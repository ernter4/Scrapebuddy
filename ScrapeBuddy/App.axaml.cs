using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ScrapeBuddy.Views;
using System.IO;

namespace ScrapeBuddy
{
    /// <summary>
    /// Represents the main application class, responsible for initializing the application and managing the lifecycle of windows.
    /// Inherits from <see cref="Application"/>.
    /// </summary>
    public class App : Application
    {
        /// <summary>
        /// Initializes the application by loading the Avalonia XAML.
        /// </summary>
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Called when the framework initialization is complete.
        /// Displays the appropriate initial window (login or passphrase creation) based on the existence of a passphrase.
        /// </summary>
        public override async void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Window initialWindow;

                if (PassphraseExists())
                {
                    initialWindow = new LoginWindow();
                }
                else
                {
                    initialWindow = new CreatePassphraseWindow();
                }

                desktop.MainWindow = initialWindow;
                initialWindow.Show();
                
                base.OnFrameworkInitializationCompleted();
            } 
        }

        /// <summary>
        /// Checks if a passphrase exists by verifying the presence and content of the configuration file.
        /// </summary>
        /// <returns>
        /// True if the passphrase file exists and contains data; otherwise, false.
        /// </returns>
        private bool PassphraseExists()
        {
            string filePath = "Data/ScrapeBuddy.conf";
            
            if (File.Exists(filePath))
            {
                string content = File.ReadAllText(filePath);
                
                return !string.IsNullOrWhiteSpace(content);
            }
            
            return false;
        } 
    }
}

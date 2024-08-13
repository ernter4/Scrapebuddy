using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ScrapeBuddy.Views;

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
        /// Displays a splash screen, initializes the main window, and closes the splash screen.
        /// </summary>
        public override async void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Create and show the splash screen (LoadingWindow)
                var splashScreen = new LoadingWindow();
                desktop.MainWindow = splashScreen;
                splashScreen.Show();

                // Simulate application initialization using the splash screen
                await splashScreen.InitApp();

                // Create and show the main window after initialization is complete
                var main = new MainWindow
                {
                    DataContext = new MainWindow(),
                };
                desktop.MainWindow = main;
                main.Show();

                // Close the splash screen
                splashScreen.Close();

                // Call the base method to complete framework initialization
                base.OnFrameworkInitializationCompleted();
            }
        }
    }
}
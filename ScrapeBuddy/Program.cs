using Avalonia;
using System;

namespace ScrapeBuddy
{
    /// <summary>
    /// The entry point for the ScrapeBuddy application.
    /// </summary>
    class Program
    {
        /// <summary>
        /// The main method for the application. Initializes the Avalonia application and starts the desktop lifetime.
        /// </summary>
        /// <param name="args">Command-line arguments passed to the application.</param>
        [STAThread]
        public static void Main(string[] args)
        {
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }

        /// <summary>
        /// Configures the Avalonia application.
        /// </summary>
        /// <returns>An <see cref="AppBuilder"/> instance configured for the application.</returns>
        public static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();
        }
    }
}

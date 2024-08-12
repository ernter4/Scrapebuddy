using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ScrapeBuddy.Views;


namespace ScrapeBuddy;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var splashScreen = new LoadingWindow();

            desktop.MainWindow = splashScreen;
            splashScreen.Show();

            await splashScreen.InitApp();

            var main = new MainWindow
            {
                DataContext = new MainWindow(),
            };

            desktop.MainWindow = main;
            main.Show();
            splashScreen.Close();

            base.OnFrameworkInitializationCompleted();
        }
    }
}
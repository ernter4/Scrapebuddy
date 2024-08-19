using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ScrapeBuddy.Views;
using ScrapeBuddy.Logic;

namespace ScrapeBuddy
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
            InitializeComponent();
        }

        /// <summary>
        /// Handles the click event for the login button.
        /// Verifies the user's password, stores the master key, and initializes the application.
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

                await splashScreen.InitApp();

                var main = new MainWindow
                {
                    DataContext = new MainWindow(),
                };

                var MainWindow = new MainWindow();
                MainWindow.Show();
                Close();

                splashScreen.Close();
            }
            else
            {
                ErrorTextBlock.Text = "Incorrect password. Please try again.";
            }
        }
    }
}

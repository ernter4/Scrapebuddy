using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using OpenQA.Selenium;
using ScrapeBuddy.Data;
using ScrapeBuddy.Logic;

namespace ScrapeBuddy.ViewModels
{
    /// <summary>
    /// Represents a user control for managing and displaying information about a registered website.
    /// </summary>
    public partial class RegisteredWebsite : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegisteredWebsite"/> class.
        /// </summary>
        public RegisteredWebsite()
        {
            
            InitializeComponent();
            SaveBtn.IsEnabled = IsInputSavable();
            WebsiteNameTextBox.TextChanged += OnTextboxInputChanged;
            WebsiteLinkTextBox.TextChanged += OnTextboxInputChanged;
            PathToElementTextBox.TextChanged += OnTextboxInputChanged;
            UsernameTextBox.TextChanged += OnTextboxInputChanged;
            PasswordTextBox.TextChanged += OnTextboxInputChanged;
            EditBtn.Click += OnEnableEdit;
            
        }

        /// <summary>
        /// Loads the website data into the user interface based on the provided ID.
        /// </summary>
        /// <param name="id">The ID of the website to load.</param>
        public void LoadWebsiteData(int id)
        {
            SetButtonState(XPathButton, false);
            SetButtonState(IdButton, true);
            DataContext = this;
            NeedsCredentials = true;
            IsEditable = false;
            EditBtn.IsChecked = false;
            
            var scrapeContent = DatabaseHandler.GetScrapeContent();
            scrapeItem = scrapeContent.FirstOrDefault(item => item.id == id);
            _websiteId = scrapeItem.id;
            _websiteName = scrapeItem.name;
            _websiteId = scrapeItem.id;
            _username = scrapeItem.username;
            _password = scrapeItem.password;
            _websiteLink = scrapeItem.url;
            XPath = scrapeItem.xPath;
            Id = scrapeItem.elementId;
            NeedsCredentials = scrapeItem.login;
            
            WebsiteNameTextBox.Text = scrapeItem.name;
            WebsiteLinkTextBox.Text = scrapeItem.url;
            
            
            if(Id == null){
                SetButtonState(XPathButton, true);
                SetButtonState(IdButton, false);
                PathToElementTextBox.Watermark = "XPath";
                Id = null;
                IsId = false;
                PathToElementTextBox.Text = scrapeItem.xPath;

            } else{
                SetButtonState(XPathButton, false);
                SetButtonState(IdButton, true);
                PathToElementTextBox.Watermark = "Id";
                XPath = null;
                PathToElementTextBox.Text = scrapeItem.elementId;
                IsId = true;

            }
            WebsiteNameTextBox.IsEnabled = false;
            WebsiteLinkTextBox.IsEnabled = false;           
            PathToElementTextBox.IsEnabled = false;
            PasswordTextBox.IsEnabled = false;
            UsernameTextBox.IsEnabled = false;
            XPathButton.IsEnabled = false;
            IdButton.IsEnabled = false;
            CredentialsBtn.IsVisible = false;
            
            if (NeedsCredentials)
            {
                CredentialsHeader.IsVisible = true;
                UsernameTextBox.IsVisible = true;
                PasswordTextBox.IsVisible = true;
                UsernameTextBox.Text = _username;
                PasswordTextBox.Text = _password;
            }
            else
            {
                CredentialsHeader.IsVisible = false;
                UsernameTextBox.IsVisible = false;
                PasswordTextBox.IsVisible = false;
            }
        }

        private string _websiteName;
        private int _websiteId;
        private string _websiteLink;
        private string? _username;
        private string? _password;
        public bool Scraping = false;
        public string Content;
        public ScrapeContent scrapeItem;
        public string? WebsiteName
        {
            get { return _websiteName; }
            set { _websiteName = value; }
        }
        public string? WebsiteLink
        {
            get { return _websiteLink; }
            set { _websiteLink = value; }
        }
        public bool IsEditable { get; set; }
        public string? XPath { get; set; }
        public string? Id { get; set; }
        public string? Username
        {
            get { return _username; }
            set { _username = value; }
        }
        public string? Password
        {
            get { return _password; }
            set { _password = value; }
        }
        public bool IsId { get; set; }
        
        public bool NeedsCredentials { get; set; }
        public bool Login;
        public event EventHandler CancelRequested;

        /// <summary>
        /// Writes user inputs to the database and raises the CancelRequested event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The event data.</param>
        private void WriteUserInputsToDatabase(object sender, RoutedEventArgs args)
        {
            Login = _password != null && _username != null;
            DatabaseHandler website = new DatabaseHandler(WebsiteName, WebsiteLink, XPath, Id, Content, Login, Username, Password);
            website.CreateWebsiteContent();
            SaveBtn.IsEnabled = false;
            LoadWebsiteData(_websiteId);
            IsEditable = false;
        }

        /// <summary>
        /// Initiates the scraping process and handles the results.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The event data.</param>
        private async void Scrape(object sender, RoutedEventArgs args)
        {
            var loadingDialog = new ScrapeDialog();
            var selenium = new Selenium();
            Login = _password != null && _username != null;
            try
            {
                loadingDialog.Show();

                IWebElement element = await Task.Run(() => selenium.Scrape(WebsiteLink, Login, XPath, Id, Username, Password));

                loadingDialog.UpdateMessage($"Scraping for {WebsiteName} succeeded: {element.Text}");
                Scraping = true;
                Content = element.Text;
                if (IsEditable)
                {
                    SaveBtn.IsEnabled = IsInputSavable();
                }
                Console.WriteLine(Scraping);
            }
            catch (Exception ex)
            {
                DatabaseHandler notification = new DatabaseHandler($"Scraping for {WebsiteName} failed: ", ex.Message);
                notification.CreateNotification();

                loadingDialog.UpdateMessage($"Scraping for {WebsiteName} failed: {ex.Message}");
            }
            finally
            {
                selenium.Quit();
                Console.WriteLine("Scraping complete");
                loadingDialog.ShowOkButton();
            }
        }

        /// <summary>
        /// Raises the CancelRequested event to notify subscribers that the view should be cancelled and the user should return to the main page.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The event data.</param>
        private void OnCancelView(object sender, RoutedEventArgs args)
        {
            CancelRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Toggles the visibility of the credentials fields in edit mode.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The event data.</param>
        private void ShowCredentialsEditView(object? sender, RoutedEventArgs routedEventArgs)
        {
            Console.WriteLine("Edit credentials: " + NeedsCredentials);
            if (NeedsCredentials)
            {
                
                UsernameTextBox.IsVisible = false;
                PasswordTextBox.IsVisible = false;
                _username = null;
                _password = null;
                
                CredentialsBtn.Content = "Add Credentials";
                NeedsCredentials = false;
                
            }
            else
            {
                UsernameTextBox.IsVisible = true;
                PasswordTextBox.IsVisible = true;
                CredentialsBtn.Content = "Remove Credentials";
                NeedsCredentials = true;

            }
        }

        /// <summary>
        /// Handles changes to text box inputs and updates corresponding properties.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The event data.</param>
        private void OnTextboxInputChanged(object sender, RoutedEventArgs args)
        {
            SaveBtn.IsEnabled = false;
            if (sender is TextBox textBox)
            {
                switch (textBox.Name)
                {
                    case "WebsiteNameTextBox":
                        WebsiteName = textBox.Text;
                        Console.WriteLine($"{WebsiteName}");
                        break;

                    case "WebsiteLinkTextBox":
                        WebsiteLink = textBox.Text;
                        Console.WriteLine($"{WebsiteLink}");
                        break;

                    case "PathToElementTextBox":
                        XPath = IsId ? null : textBox.Text;
                        Id = IsId ? textBox.Text : null;
                        break;

                     case "PasswordTextBox":
                        Password = textBox.Text;
                        Console.WriteLine($"{Password}");
                        break;
                   
                     case "UsernameTextBox": 
                        Username = textBox.Text;
                        Console.WriteLine($"{Username}");
                        break;



                    default:
                        Console.WriteLine("Unknown Textbox");
                        break;
                }
            }
        }

        /// <summary>
        /// Checks if all conditions for user input to be savable are met.
        /// </summary>
        /// <returns>True if the input is savable; otherwise, false.</returns>
        public bool IsInputSavable()
        {
            return (!string.IsNullOrWhiteSpace(_websiteName) && Scraping && !string.IsNullOrWhiteSpace(_websiteLink) && (!string.IsNullOrWhiteSpace(Id) || !string.IsNullOrWhiteSpace(XPath)));
        }

        /// <summary>
        /// Toggles the edit mode of the text boxes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The event data.</param>
        public void OnEnableEdit(object? sender, EventArgs args)
        {
            IsEditable = !IsEditable;
            Console.WriteLine(IsEditable);
            WebsiteNameTextBox.IsEnabled = IsEditable;
            WebsiteLinkTextBox.IsEnabled = IsEditable;
            PathToElementTextBox.IsEnabled = IsEditable;
            XPathButton.IsEnabled = IsEditable;
            IdButton.IsEnabled = IsEditable;
            UsernameTextBox.IsEnabled = IsEditable;
            PasswordTextBox.IsEnabled = IsEditable;
            CredentialsBtn.IsVisible = IsEditable;
            if (NeedsCredentials)
            {
                PasswordTextBox.IsEnabled = IsEditable;
                UsernameTextBox.IsEnabled = IsEditable;
                CredentialsHeader.IsVisible = !IsEditable;
            }
            else
            {
                PasswordTextBox.IsVisible = false;
                UsernameTextBox.IsVisible = false;
            }

            if (!IsEditable)
            {
                LoadWebsiteData(_websiteId);
            }
        }

        /// <summary>
        /// Sets the XPath button as active and ID button as inactive, and updates the watermark of the PathToElementTextBox.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void OnXPathClicked(object sender, RoutedEventArgs e)
        {
            SetButtonState(XPathButton, true);
            SetButtonState(IdButton, false);
            PathToElementTextBox.Watermark = "XPath"; 
            PathToElementTextBox.Text = XPath;
            XPath = PathToElementTextBox.Text;
            Id = null;
            IsId = false;
        }

        /// <summary>
        /// Sets the ID button as active and XPath button as inactive, and updates the watermark of the PathToElementTextBox.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void OnIDClicked(object sender, RoutedEventArgs e)
        {
            SetButtonState(XPathButton, false);
            SetButtonState(IdButton, true);
            PathToElementTextBox.Watermark = "Id";
            PathToElementTextBox.Text = Id;
            XPath = null;
            Id = PathToElementTextBox.Text;
            IsId = true;
        }

        /// <summary>
        /// Updates the visual state of the button based on whether it is active or not.
        /// </summary>
        /// <param name="button">The button to update.</param>
        /// <param name="isActive">Indicates if the button should be active.</param>
        private void SetButtonState(Button button, bool isActive)
        {
            if (isActive)
            {
                button.Background = Brushes.DodgerBlue;
                button.Foreground = Brushes.White;
                button.BorderBrush = Brushes.DodgerBlue;
            }
            else
            {
                button.Background = new SolidColorBrush(Color.Parse("#f5f7fa"));
                button.Foreground = Brushes.Black;
                button.BorderBrush = Brushes.Black;
            }
        }
    }
}

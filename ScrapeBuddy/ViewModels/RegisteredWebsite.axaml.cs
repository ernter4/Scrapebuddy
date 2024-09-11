using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using OpenQA.Selenium;
using ScrapeBuddy.Data;
using ScrapeBuddy.Logic;
using Newtonsoft.Json.Linq;
using System.IO;

namespace ScrapeBuddy.ViewModels
{
    /// <summary>
    /// Represents a user control for managing and displaying information about a registered website.
    /// </summary>
    public partial class RegisteredWebsite : UserControl
    {
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
        private int clickCount = 0;
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
        private List<string>? ClicksList;
        public event EventHandler CancelRequested;
        private MainWindow mainPageView;


        /// <summary>
        /// Initializes a new instance of the <see cref="RegisteredWebsite"/> class.
        /// </summary>
        /// <param name="mainPageView"></param>
        public RegisteredWebsite(MainWindow mainPageView)
        {
            InitializeComponent();
            SettingsViewModel.ThemeChanged += SetTheme;
            SetTheme(null, null);
            SaveBtn.IsEnabled = IsInputSavable();
            WebsiteNameTextBox.TextChanged += OnTextboxInputChanged;
            WebsiteLinkTextBox.TextChanged += OnTextboxInputChanged;
            PathToElementTextBox.TextChanged += OnTextboxInputChanged;
            UsernameTextBox.TextChanged += OnTextboxInputChanged;
            PasswordTextBox.TextChanged += OnTextboxInputChanged;
            EditBtn.Click += OnEnableEdit;
            mainPageView = mainPageView;
        }

        /// <summary>
        /// Loads the website data into the user interface based on the provided ID.
        /// </summary>
        /// <param name="id">The ID of the website to load.</param>
        public void LoadWebsiteData(int id)
        {
            ClicksStackPanel.Children.Clear();
            SetButtonState(XPathButton, false);
            SetButtonState(IdButton, true);
            DataContext = this;
            NeedsCredentials = true;
            IsEditable = false;
            EditBtn.IsChecked = false;
            SaveBtn.IsVisible = false;
            ContentTextBox.IsVisible = true;
            clickCount = 0;
            ToolTip.SetTip(SaveBtn, "Scrape to save your inputs");
            
            var scrapeContent = DatabaseHandler.GetScrapeContent();
            scrapeItem = scrapeContent.FirstOrDefault(item => item.id == id);
            _websiteId = scrapeItem.id;
            _websiteName = scrapeItem.name;
            _websiteId = scrapeItem.id;
            _username = scrapeItem.username;
            _password = scrapeItem.password;
            _websiteLink = scrapeItem.url;
            ClicksList = scrapeItem.clicks;
            XPath = scrapeItem.xPath;
            Id = scrapeItem.elementId;
            NeedsCredentials = scrapeItem.login;

            if (ClicksList == null || ClicksList.Count == 0)
            {
                ClicksText.IsVisible = false;
            }
            else
            {
                ClicksText.IsVisible = true;
            }
            
            WebsiteNameTextBox.Text = scrapeItem.name;
            WebsiteLinkTextBox.Text = scrapeItem.url;
            ContentTextBox.Text = scrapeItem.content;
            
            DisplayClicks();
            ShowDeleteClicks(false);
            SetAllTextBoxesEditability(false);
            
            
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
            ClicksButton.IsVisible = false;
            
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
        
        /// <summary>
        /// Writes user inputs to the database and raises the CancelRequested event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The event data.</param>
        private void WriteUserInputsToDatabase(object? sender, RoutedEventArgs args)
        {
            Login = _password != null && _username != null;
            DatabaseHandler website = new DatabaseHandler(WebsiteName, WebsiteLink, ClicksList, XPath, Id, Content, Login, Username, Password);
            website.UpdateWebsite(_websiteId);
            SaveBtn.IsEnabled = false;
            LoadWebsiteData(_websiteId);
            IsEditable = false;
            mainPageView.UpdateList();
        }

        /// <summary>
        /// Initiates the scraping process and handles the results.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The event data.</param>
        private async void Scrape(object sender, RoutedEventArgs args)
        {
            var loadingDialog = new ScrapeDialog();
            StoreClicksData();
            var selenium = new Selenium();
            
            Login = !string.IsNullOrEmpty(PasswordTextBox.Text) && !string.IsNullOrEmpty(UsernameTextBox.Text);
            Console.WriteLine("Login:" + Login + _password + _username);
            try
            {
                loadingDialog.Show();

                IWebElement element = await Task.Run(() => selenium.Scrape(WebsiteLink, Login, ClicksList, XPath, Id, Username, Password));

                loadingDialog.UpdateMessage($"Scraping for {WebsiteName} succeeded: {element.Text}", true);
                if (element.Text == scrapeItem.content)
                {
                    Console.WriteLine($"Content is still the same: {element.Text} = {scrapeItem.content}");
                }
                else
                {
                    DatabaseHandler.UpdateContent(_websiteId, element.Text);
                    SettingsManager _settingsManager = new SettingsManager();
                    SettingsModel _settings = _settingsManager.LoadSettings();
                }

                Scraping = true;
                Content = element.Text;
                ContentTextBox.Text = element.Text;
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

                loadingDialog.UpdateMessage($"Scraping for {WebsiteName} failed: {ex.Message}", false);
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
                        break;

                    case "WebsiteLinkTextBox":
                        WebsiteLink = textBox.Text;
                        break;

                    case "PathToElementTextBox":
                        XPath = IsId ? null : textBox.Text;
                        Id = IsId ? textBox.Text : null;
                        break;

                     case "PasswordTextBox":
                        Password = textBox.Text;
                        break;
                   
                     case "UsernameTextBox": 
                        Username = textBox.Text;
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
            SaveBtn.IsVisible = IsEditable;
            WebsiteLinkTextBox.IsEnabled = IsEditable;
            PathToElementTextBox.IsEnabled = IsEditable;
            XPathButton.IsEnabled = IsEditable;
            IdButton.IsEnabled = IsEditable;
            UsernameTextBox.IsEnabled = IsEditable;
            PasswordTextBox.IsEnabled = IsEditable;
            CredentialsBtn.IsVisible = IsEditable;
            ClicksButton.IsVisible = IsEditable;
            ContentTextBox.IsVisible = !IsEditable;
            ContentText.IsVisible = !IsEditable;
            ClicksText.IsVisible = IsEditable;
            
            if (NeedsCredentials)
            {
                PasswordTextBox.IsEnabled = IsEditable;
                UsernameTextBox.IsEnabled = IsEditable;
                CredentialsHeader.IsVisible = !IsEditable;
                CredentialsBtn.Content = "Remove Credentials";
            }
            else
            {
                PasswordTextBox.IsVisible = false;
                UsernameTextBox.IsVisible = false;
                CredentialsBtn.Content = "Add Credentials";
                PasswordTextBox.Text = string.Empty;
                UsernameTextBox.Text = string.Empty;
            }

            if (!IsEditable)
            {
                LoadWebsiteData(_websiteId);
            }

            ShowDeleteClicks(IsEditable);
            SetAllTextBoxesEditability(IsEditable);
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
        /// Handles the click event of the ClicksButton. (Add clicks)
        /// Creates a new <see cref="DockPanel"/> containing a <see cref="TextBox"/> and a delete <see cref="Button"/>.
        /// The <see cref="TextBox"/> is initialized with a unique watermark indicating the click count,
        /// and the <see cref="Button"/> allows for removing the <see cref="DockPanel"/> from the <see cref="ClicksStackPanel"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void OnClicksButtonClick(object sender, RoutedEventArgs e)
        {
            clickCount++;
            
            var dockPanel = new DockPanel
            {
                Margin = new Thickness(10, 5, 0, 0)
            };
            var textBox = new TextBox
            {
                Name = $"ClicksTextBox{clickCount}",
                Watermark = $"XPath Click {clickCount}",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                Width = 100,
            };
            
            dockPanel.Children.Add(textBox);
            DockPanel.SetDock(textBox, Dock.Left);

            var deleteButton = new Button
            {
                Name = $"DeleteButton{clickCount}",
                Content = "X",
                Margin = new Thickness(5, 0, 0, 0),
                Classes = {"btn_dark"},
                
            };
            deleteButton.Click += (s, args) => ClicksStackPanel.Children.Remove(dockPanel);
            dockPanel.Children.Add(deleteButton);
            DockPanel.SetDock(deleteButton, Dock.Right);
            ClicksStackPanel.Children.Add(dockPanel);
        }
        
        /// <summary>
        /// Stores the current data from all <see cref="TextBox"/> controls contained within <see cref="ClicksStackPanel"/> into the <see cref="ClicksList"/>.
        /// Clears the existing entries in <see cref="ClicksList"/> before populating it with the current <see cref="TextBox"/> contents.
        /// </summary>
        private void StoreClicksData()
        {
            if (ClicksList == null)
            {
                return; 
            }
            ClicksList.Clear();
            foreach (var child in ClicksStackPanel.Children)
            {
                if (child is DockPanel dockPanel)
                {
                    var textBox = dockPanel.Children.OfType<TextBox>().FirstOrDefault();
                    if (textBox != null)
                    {
                        ClicksList.Add(textBox.Text);
                    }
                }
            }
        }
        
        /// <summary>
        /// Sets the editability of all TextBox controls within ClicksStackPanel.
        /// </summary>
        /// <param name="isEditable">True to make the TextBox controls editable; false to make them read-only.</param>
        private void SetAllTextBoxesEditability(bool isEditable)
        {
            foreach (var child in ClicksStackPanel.Children)
            {
                if (child is DockPanel dockPanel)
                {
                    foreach (var panelChild in dockPanel.Children)
                    {
                        if (panelChild is TextBox textBox)
                        {
                            textBox.IsEnabled = isEditable;
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Toggles the visibility of all delete buttons in the <see cref="ClicksStackPanel"/> based on the provided flag.
        /// </summary>
        /// <param name="isEnabled">A boolean value indicating whether the delete buttons should be visible or not. 
        /// <c>true</c> to make the delete buttons visible; <c>false</c> to hide them.</param>
        private void ShowDeleteClicks(bool isEnabled)
        {
            foreach (var child in ClicksStackPanel.Children)
            {
                if (child is DockPanel dockPanel)
                {
                    var deleteButton = dockPanel.Children.OfType<Button>()
                        .FirstOrDefault(b => b.Name.StartsWith("DeleteButton"));
                    if (deleteButton != null)
                    {
                        deleteButton.IsVisible = isEnabled;
                    }
                }
            }
        }
        
        /// <summary>
        /// Displays the clicks stored in <see cref="ClicksList"/> as <see cref="TextBox"/> controls within the <see cref="ClicksStackPanel"/>. 
        /// Each click is represented by a <see cref="TextBox"/> and a corresponding delete <see cref="Button"/>.
        /// </summary>
        private void DisplayClicks()
        {
            if (ClicksList != null)
            {
                foreach (var click in ClicksList)
                {
                    clickCount++;

                    // Create a new DockPanel to hold the TextBox and Button
                    var dockPanel = new DockPanel
                    {
                        Margin = new Thickness(10, 5, 0, 0)
                    };

                    // Create the TextBox
                    var textBox = new TextBox
                    {
                        Name = $"ClicksTextBox{clickCount}",
                        Text = click,
                        Watermark = $"XPath",
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                        Width = 100,
                    };

                    // Add TextBox to the DockPanel
                    dockPanel.Children.Add(textBox);
                    DockPanel.SetDock(textBox, Dock.Left);

                    var deleteButton = new Button
                    {
                        Name = $"DeleteButton{clickCount}", // Set a unique name
                        Content = "X",
                        Margin = new Thickness(5, 0, 0, 0),
                        Classes = {"btn_dark"},
                    };

                    // Handle delete button click event
                    deleteButton.Click += (s, args) => ClicksStackPanel.Children.Remove(dockPanel);

                    // Add Button to the DockPanel
                    dockPanel.Children.Add(deleteButton);
                    DockPanel.SetDock(deleteButton, Dock.Right);

                    // Add the DockPanel to ClicksStackPanel
                    ClicksStackPanel.Children.Add(dockPanel);
                }
            }
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
        
        /// <summary>
        /// Determines whether user inputs can be written to the database.
        /// </summary>
        /// <param name="parameter">An optional parameter that may influence the ability to write to the database.</param>
        /// <param name="reporter">An object used to report why the action cannot be executed.</param>
        /// <returns>
        /// <c>true</c> if user inputs can be written to the database; otherwise, <c>false</c>.
        /// </returns>
        internal bool CanWriteUserInputsToDatabase(object? parameter, IReportReasonCantExecute reporter)
        {
            if (!SaveBtn.IsEnabled)
            {
                reporter.ReasonCantExecute = "You can't do the thing yet.";
                return false;
            }

            return true;
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
                RootControl.Foreground = Brushes.White;
                MainGrid.Background = (SolidColorBrush)Application.Current.Resources["BackgroundDark"];
                ScrapeBtn.Classes.Remove("btn_dark");
                ScrapeBtn.Classes.Add("btn_white");
                SaveBtn.Classes.Remove("btn_dark");
                SaveBtn.Classes.Add("btn_white");
                HomeBtn.Classes.Remove("btn_dark");
                HomeBtn.Classes.Add("btn_white");
                EditBtn.Classes.Remove("btn_dark");
                EditBtn.Classes.Add("btn_white");
                ClicksButton.Classes.Remove("btn_dark");
                ClicksButton.Classes.Add("btn_white");
                CredentialsBtn.Classes.Remove("btn_dark");
                CredentialsBtn.Classes.Add("btn_white");

            }
            else
            {
                RootControl.Foreground = Brushes.Black;
                MainGrid.Background = (SolidColorBrush)Application.Current.Resources["BackgroundLight"]; ;
                ScrapeBtn.Classes.Remove("btn_white");
                ScrapeBtn.Classes.Add("btn_dark");
                SaveBtn.Classes.Remove("btn_white");
                SaveBtn.Classes.Add("btn_dark");
                HomeBtn.Classes.Remove("btn_white");
                HomeBtn.Classes.Add("btn_dark");
                EditBtn.Classes.Remove("btn_white");
                EditBtn.Classes.Add("btn_dark");
                ClicksButton.Classes.Remove("btn_white");
                ClicksButton.Classes.Add("btn_dark");
                CredentialsBtn.Classes.Remove("btn_white");
                CredentialsBtn.Classes.Add("btn_dark");
            }
        }
    }
}
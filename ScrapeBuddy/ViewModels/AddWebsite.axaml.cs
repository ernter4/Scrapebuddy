using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using ScrapeBuddy.Data;
using ScrapeBuddy.Logic;
using System.IO;


namespace ScrapeBuddy.ViewModels;

/// <summary>
/// Represents the user control for managing a new website entry, including functionality for saving user inputs and switching between XPath and Id modes.
/// </summary>
public partial class AddWebsite : UserControl
{
    public AddWebsite()
    {
        InitializeComponent();
        DataContext = this;
        NeedsCredentials = false;
        SaveBtn.IsEnabled = false;
        ToolTip.SetTip(SaveBtn, "Scrape to save your inputs");
        WebsiteNameTextBox.TextChanged += OnTextboxInputChanged;
        WebsiteLinkTextBox.TextChanged += OnTextboxInputChanged;
        PathToElementTextBox.TextChanged += OnTextboxInputChanged;
        UsernameTextBox.TextChanged += OnTextboxInputChanged;
        PasswordTextBox.TextChanged += OnTextboxInputChanged;
        SetButtonState(XPathButton, false);
        SetButtonState(IdButton, true); SettingsViewModel.ThemeChanged += SetTheme;
        SetTheme(null, null);
        
    }

    private string _websiteName;
    private string _websiteLink;
    private string _username;
    private string _password; 
    public string? WebsiteName
    {
        get { return _websiteName; }
        set { _websiteName = value; }
    }
    public string? WebsiteLink
    {
        get { return _websiteLink;}
        set { _websiteLink = value; }
    }
    public string? XPath { get; set; }
    public string? Id { get; set; }
    public string? Username
    {
        get { return _username;}
        set { _username = value; }
    } 
    public string? Password
    {
        get { return _password;}
        set { _password = value; }
    }
    public bool IsId { get; set; }
    public bool Login = false;
    public bool Scraping = false;
    public string Content;
    private int clickCount = 0;
    public bool NeedsCredentials { get; set; }
    private List<string>? ClicksList = new List<string>();
    private Style PseudoDeactivated { get; set; }

    public event EventHandler CancelRequested;

    /// <summary>
    /// Clears all input fields.
    /// </summary>
    private void ClearAllFields()
    {
        WebsiteName = null;
        WebsiteNameTextBox.Text = null;
        WebsiteLink = null;
        WebsiteLinkTextBox.Text = null;
        XPath = null;
        Id = null;
        PathToElementTextBox.Text = null;
        Username = null;
        UsernameTextBox.Text = null;
        Password = null;
        PasswordTextBox.Text = null;
        Scraping = false;
        Login = false;
        clickCount = 0;
        ClicksStackPanel.Children.Clear();
    }

    /// <summary>
    /// Writes user inputs to the database and clears fields.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">The event data.</param>
    private void WriteUserInputsToDatabase(object? sender, RoutedEventArgs args)
    {
        Login = _password != null && _username != null;
        DatabaseHandler website = new DatabaseHandler(WebsiteName, WebsiteLink, ClicksList, XPath, Id, Content, Login, Username, Password);
        website.CreateWebsiteContent();
        CancelRequested.Invoke(this, EventArgs.Empty);
        ClearAllFields();
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
        Login = _password != null && _username != null;

        try
        {
            loadingDialog.Show();
            IWebElement element = await Task.Run(() => selenium.Scrape(WebsiteLink, Login, ClicksList, XPath, Id, Username, Password));
            loadingDialog.UpdateMessage($"Scraping for {WebsiteName} succeeded: {element.Text}", true);
            Scraping = true;
            Content = element.Text;
            SaveBtn.IsEnabled = true;
            ToolTip.SetTip(SaveBtn, AvaloniaProperty.UnsetValue);
        }
        catch (Exception ex)
        {
            DatabaseHandler notification = new DatabaseHandler($"Scraping for {WebsiteName} failed: ", ex.Message);
            notification.CreateNotification();

            loadingDialog.UpdateMessage($"Scraping for {WebsiteName} failed: {ex.Message}", false);
            SaveBtn.IsEnabled = false;
            ToolTip.SetTip(SaveBtn, "Scrape to save your inputs");
        }
        finally
        {
            selenium.Quit();
            Console.WriteLine("Scraping complete");
            loadingDialog.ShowOkButton();
        }
    }

    /// <summary>
    /// Cancels the view and raises the CancelRequested event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">The event data.</param>
    private void OnCancelView(object sender, RoutedEventArgs args)
    {
        CancelRequested.Invoke(this, EventArgs.Empty);
        ClearAllFields();
    }

    /// <summary>
    /// Clears all input fields when invoked.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">The event data.</param>
    private void ClearFields(object sender, RoutedEventArgs args)
    {
        ClearAllFields();
    }

    /// <summary>
    /// Toggles the visibility of the credentials fields.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">The event data.</param>
    private void ShowCredentialsView(object sender, RoutedEventArgs args)
    {
        NeedsCredentials = !NeedsCredentials;
        if (NeedsCredentials)
        {
            CredentialsBtn.Content = "Remove Credentials";
        }
        else
        {
            CredentialsBtn.Content = "Add Credentials";
        }
        Password = null;
        Username = null;
        Console.WriteLine($"{NeedsCredentials}");
        UsernameBoxContainer.IsVisible = NeedsCredentials;
        PasswordBoxContainer.IsVisible = NeedsCredentials;
        UsernameTextBox.Text = null;
        PasswordTextBox.Text = null;
    }

    /// <summary>
    /// Updates the corresponding variables when text box input changes and checks if the input is savable.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">The event data.</param>
    private void OnTextboxInputChanged(object sender, RoutedEventArgs args)
    {
        SaveBtn.IsEnabled = false;
        ToolTip.SetTip(SaveBtn, "Scrape to save your inputs");
        if (sender is TextBox textBox)
        {
            Console.WriteLine(textBox.Name);
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
    /// Checks if user input is valid and savable.
    /// </summary>
    /// <returns>True if savable; otherwise, false.</returns>
    public bool IsInputSavable()
    {
        return (!string.IsNullOrWhiteSpace(_websiteName) && Scraping && !string.IsNullOrWhiteSpace(_websiteLink) && (!string.IsNullOrWhiteSpace(Id) || !string.IsNullOrWhiteSpace(XPath)));
    }

    /// <summary>
    /// Handles the click event for the XPath button and updates the state.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void OnXPathClicked(object sender, RoutedEventArgs e)
    {
        SetButtonState(XPathButton, true);
        SetButtonState(IdButton, false);
        PathToElementTextBox.Watermark = "XPath";
        //XPath = PathToElementTextBox.Text;
        //Id = null;
        IsId = false;
    }

    /// <summary>
    /// Handles the click event for the ID button and updates the state.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void OnIDClicked(object sender, RoutedEventArgs e)
    {
        SetButtonState(XPathButton, false);
        SetButtonState(IdButton, true);
        PathToElementTextBox.Watermark = "Id";
        //XPath = null;
        //Id = PathToElementTextBox.Text;
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
        if (clickCount <= 4)
        {
            var dockPanel = new DockPanel
            {
                Margin = new Thickness(0, 5, 0, 0)
            };
        
            var textBox = new TextBox
            {
                Name = $"ClicksTextBox{clickCount}",
                Watermark = $"XPath Click",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                Margin = new Thickness(5, 0, 0, 0)
            };
            dockPanel.Children.Add(textBox);
            DockPanel.SetDock(textBox, Dock.Left);
            var deleteButton = new Button
            {
                Name = $"DeleteButton{clickCount}", 
                Content = "X",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                Margin = new Thickness(2, 0, 0, 0)
            };
            deleteButton.Click += (s, args) => ClicksStackPanel.Children.Remove(dockPanel);
            dockPanel.Children.Add(deleteButton);
            DockPanel.SetDock(deleteButton, Dock.Right);
        
            ClicksStackPanel.Children.Add(dockPanel);
        }
        else
        {
            Console.WriteLine("More than 4 Clicks cannot be added");
        }
    }
    
    /// <summary>
    /// Stores the current data from all <see cref="TextBox"/> controls contained within <see cref="ClicksStackPanel"/> into the <see cref="ClicksList"/>.
    /// Clears the existing entries in <see cref="ClicksList"/> before populating it with the current <see cref="TextBox"/> contents.
    /// </summary>
    private void StoreClicksData()
    {
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
    /// Updates the visual state of a button based on its active status.
    /// </summary>
    /// <param name="button">The button to update.</param>
    /// <param name="isActive">Whether the button is active or not.</param>
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
            ClearBtn.Classes.Remove("btn_dark");
            ClearBtn.Classes.Add("btn_white");
            ClicksButton.Classes.Remove("btn_dark");
            ClicksButton.Classes.Add("btn_white");
            CredentialsBtn.Classes.Remove("btn_dark");
            CredentialsBtn.Classes.Add("btn_white");

        }
        else
        {
            RootControl.Foreground = Brushes.Black;
            MainGrid.Background = (SolidColorBrush)Application.Current.Resources["BackgroundLight"];
            ScrapeBtn.Classes.Remove("btn_white");
            ScrapeBtn.Classes.Add("btn_dark");
            SaveBtn.Classes.Remove("btn_white");
            SaveBtn.Classes.Add("btn_dark");
            HomeBtn.Classes.Remove("btn_white");
            HomeBtn.Classes.Add("btn_dark");
            ClearBtn.Classes.Remove("btn_white");
            ClearBtn.Classes.Add("btn_dark");
            ClicksButton.Classes.Remove("btn_white");
            ClicksButton.Classes.Add("btn_dark");
            CredentialsBtn.Classes.Remove("btn_white");
            CredentialsBtn.Classes.Add("btn_dark");
        }
    }
    
}

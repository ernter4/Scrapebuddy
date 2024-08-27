using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;

namespace ScrapeBuddy.ViewModels;

public partial class RegisteredWebsite : UserControl
{
    public RegisteredWebsite()
    {
        DataContext = this;
        NeedsCredentials = true;
        IsEditable = false;
        InitializeComponent();
        SaveBtn.IsEnabled = IsInputSavable();
        WebsiteNameTextBox.TextChanged += OnTextboxInputChanged;
        WebsiteLinkTextBox.TextChanged += OnTextboxInputChanged;
        PathToElementTextBox.TextChanged += OnTextboxInputChanged;
        UsernameTextBox.TextChanged += OnTextboxInputChanged;
        PasswordTextBox.TextChanged += OnTextboxInputChanged;
        EditBtn.Click += OnEnableEdit;
        SetButtonState(XPathButton, false);
        SetButtonState(IdButton, true);
    }
    
    private string _websiteName;
    private string _websiteLink;
    private string _username;
    private string _password;
    
    /// <summary>
    /// Gets or sets the name of the website.
    /// </summary>
    public string? WebsiteName
    {
        get { return _websiteName; }
        set { _websiteName = value; }
    }
    /// <summary>
    /// Gets or sets the URL link of the website.
    /// </summary>
    public string? WebsiteLink
    {
        get { return _websiteLink;}
        set { _websiteLink = value; }
    }
    
    public bool IsEditable { get; set; }
    
    public string? XPath { get; set; }

    public string? Id { get; set; }
    
    /// <summary>
    /// Gets or sets the Username
    /// </summary>
    public string? Username {  
        get { return _username;}
        set { _username = value; } 
    }
    
    /// <summary>
    /// Gets or sets the Password
    /// </summary>
    public string? Password
    {
        get { return _password;}
        set { _password = value; }
    }
    /// <summary>
    /// Gets or sets a value indicating whether the website uses Id for identification.
    /// </summary>
    public bool IsId
    {
        get;
        set;
    }
    private List<string?> StoredUserWebsites { get; set; }
    
    
    /// <summary>
    /// gets or sets a value wether the user needs to authorize for a website
    /// </summary>
    public bool NeedsCredentials { get; set; }
    /// <summary>
    /// Gets or sets the style to apply when the control is pseudo-deactivated.
    /// </summary>
    private Style PseudoDeactivated { get; set; }
    /// <summary>
    /// Occurs when the cancel action is requested.
    /// </summary>
    public event EventHandler CancelRequested;
    
    /// <summary>
    /// sets all inputfields to null
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void ClearAllFields(object sender, RoutedEventArgs args)
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
    }
    
    private void WriteUserInputsToDatabase(object sender)
    {
        
    }
    
    /// <summary>
    /// prints every user input related variable
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void PrintAllVariables(object sender, RoutedEventArgs args)
    {
        Console.WriteLine($"WebsiteName: {WebsiteName} WebsiteLink: {WebsiteLink} XPath: {XPath} Id: {Id} Username: {Username} Password: {Password}");
    }
    /// <summary>
    /// Raises the CancelRequested event to notify subscribers that the view should be cancelled and the user should return to the main page.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">The event data.</param>
    private void OnCancelView(object sender, RoutedEventArgs args)
    {
        CancelRequested.Invoke(this, EventArgs.Empty);
    }
    /// <summary>
    /// Applies or removes a pseudo-activity style to the given control based on the specified state.
    /// </summary>
    /// <param name="control">The control to which the style will be applied or removed.</param>
    /// <param name="setInactive">A boolean value indicating whether to set the control as inactive (apply style) or active (remove style).</param>
    private void SetPseudoActivityStyle(Control control, bool setInactive)
    {
        if (setInactive)
        {
            PseudoDeactivated = new Style()
            {
                Setters =
                {
                    new Setter()
                    {
                        Property = OpacityProperty,
                        Value = 0.5
                    }
                }
            };
            control.Styles.Add(PseudoDeactivated);
        }
        else
        {
            if (PseudoDeactivated != null)
            {
                control.Styles.Remove(PseudoDeactivated);
            }
        }
    }
    
    /// <summary>
    /// makes Credentials visible/ invisible
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">The event data.</param>
    private void ShowCredentialsView(object sender, RoutedEventArgs args)
    {
        NeedsCredentials = !NeedsCredentials;
        Console.WriteLine($"{NeedsCredentials}");
        UsernameTextBox.IsVisible = NeedsCredentials;
        PasswordTextBox.IsVisible = NeedsCredentials;
    }


    /// <summary>
    /// saves Userinput to dedicated variables and checks if the content is savable
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void OnTextboxInputChanged(object sender, RoutedEventArgs args)
    {
        if (sender is TextBox textBox)
        {
            Console.WriteLine(textBox.Name);
            switch (textBox.Name)
            {   
                case "WebsiteNameTextBox":
                    WebsiteName = textBox.Text;
                    Console.WriteLine($"{WebsiteName}");
                    SaveBtn.IsEnabled = IsInputSavable();
                    break;
                   
                case "WebsiteLinkTextBox":
                    WebsiteLink = textBox.Text;
                    Console.WriteLine($"{WebsiteLink}");
                    SaveBtn.IsEnabled = IsInputSavable();
                    break;
                   
                case "PathToElementTextBox":
                    XPath = IsId ? null : textBox.Text;
                    Id = IsId ? textBox.Text : null;
                    SaveBtn.IsEnabled = IsInputSavable();
                    break;
                   
                case "PasswordTextBox":
                    Password = textBox.Text;
                    Console.WriteLine($"{Password}");
                    SaveBtn.IsEnabled = IsInputSavable();
                    break;
                   
                case "UsernameTextBox": 
                    Username = textBox.Text;
                    Console.WriteLine($"{Username}");
                    SaveBtn.IsEnabled = IsInputSavable();
                    break;
                   
                default:
                    Console.WriteLine("Unknown Textbox");
                    break;
            }
        }
    }

    /// <summary>
    /// checks if all conditions for userinput to be savable are true
    /// </summary>
    /// <returns></returns>
    public bool IsInputSavable()
    {
        return (!string.IsNullOrWhiteSpace(_websiteName) && !string.IsNullOrWhiteSpace(_websiteLink) && (!string.IsNullOrWhiteSpace(Id) || !string.IsNullOrWhiteSpace(XPath)));
    }

    public void OnEnableEdit(object? sender, EventArgs args)
    {
        IsEditable = !IsEditable;
        WebsiteNameTextBox.IsEnabled = IsEditable;
        WebsiteLinkTextBox.IsEnabled = IsEditable;
        PathToElementTextBox.IsEnabled = IsEditable;
        PasswordTextBox.IsEnabled = IsEditable;
        UsernameTextBox.IsEnabled = IsEditable;
    }
    
    private void OnXPathClicked(object sender, RoutedEventArgs e)
    {
        SetButtonState(XPathButton, true);
        SetButtonState(IdButton, false);
        PathToElementTextBox.Watermark = "XPath";
        XPath = PathToElementTextBox.Text;
        Id = null;
        IsId = false;
    }

    private void OnIDClicked(object sender, RoutedEventArgs e)
    {
        SetButtonState(XPathButton, false);
        SetButtonState(IdButton, true);
        PathToElementTextBox.Watermark = "Id";
        XPath = null;
        Id = PathToElementTextBox.Text;
        IsId = true;
    }
       
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
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V125.IndexedDB;

namespace ScrapeBuddy.Logic;

public class Selenium
{
    private IWebDriver _driver;
    /*!
     * \brief constructor of class.
     *
     */
    public Selenium(string url)
    {
        var options = new ChromeOptions();
        options.AddArgument("--disable-search-engine-choice-screen");
        options.AddArgument("--headless"); // Add headless mode
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        _driver = new ChromeDriver(options);

        _driver.Navigate().GoToUrl(url);
    } 
    /*!
     * \brief returns Element specified by XPath.
     * \param xpath The XPath by wich an Element can be identified.
     */
    public  IWebElement GetElementByXPath(string xpath)
    {
        return _driver.FindElement(By.XPath(xpath));
    }
    /*!
     * \brief returns Element specified by ID.
     * \param id The id by wich an Element can be identified.
     */
    public  IWebElement GetElementById(string id)
    {
        return _driver.FindElement(By.Id(id));
    }

    
    /*!
     * \brief Logs into an Website by autocomplete feature 
     * \param username  A String used for authanticating.
     * \param password A String used to authenticate user.
     * \param bool Decides if password and username can be filled in at the same Time. Some Websites as Google only show the password field after you inserted the username. 
     */
    public void login(string username, string password, bool multiple)
    {
        
        IWebElement usernamefield = _driver.FindElement(By.CssSelector("[autocomplete='username']"));
        IWebElement passwordfield;
       usernamefield.SendKeys(username);
       if (multiple)
       {
           usernamefield.SendKeys(Keys.Return);
           
       }
       passwordfield = _driver.FindElement(By.CssSelector("[autocomplete='current-password']"));
       
       passwordfield.SendKeys(password);
       
       passwordfield.SendKeys(Keys.Return);
       
       
    }
    
    /*
    * \brief quits the browser.

    */
    public void Quit()
    {
        _driver.Quit();
    }
    
}


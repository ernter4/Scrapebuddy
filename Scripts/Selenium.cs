using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

namespace ScrapeBuddy.Scripts;

public class Selenium
{
    private IWebDriver _driver;
    /*
     * \brief constructor of class 
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
    /*
     * \brief returns Element specified by XPath.
     * @param xpath The XPath by wich an Element can be identified.
     */
    public  IWebElement GetElementByXPath(string xpath)
    {
        return _driver.FindElement(By.XPath(xpath));
    }
    /*
     * \brief returns Element specified by ID.
     * @param id The id by wich an Element can be identified.
     */
    public  IWebElement GetElementById(string id)
    {
        return _driver.FindElement(By.Id(id));
    }

    /*
    * \brief quits the browser.

    */
    public void Quit()
    {
        _driver.Quit();
    }
}


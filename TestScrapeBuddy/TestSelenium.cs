using ScrapeBuddy.Scripts;
namespace TestScrapeBuddy;

public class SeleniumTest
{
    private Selenium? _driver;

    [SetUp]
    public void Setup()
    {

        _driver = new Selenium("https://dualis.dhbw.de");
        Thread.Sleep(1000);
    }

    [Test]
    public void XPath()
    {
        
        String title = _driver!.GetElementByXPath("//*[@id=\"link000280\"]").GetAttribute("Title");
        _driver.Quit();
        Assert.That(title, Is.EqualTo("Kontakt")); 
    }
    [Test]
    public void Id()
    {
        String title = _driver!.GetElementById("link000340").GetAttribute("Title");
        _driver.Quit();
        Assert.That(title, Is.EqualTo("Informationen")); 
        

    }
    
 
}

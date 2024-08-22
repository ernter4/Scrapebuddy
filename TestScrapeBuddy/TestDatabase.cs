using ScrapeBuddy.Data;
using ScrapeBuddy.Logic;

namespace TestScrapeBuddy;

[TestFixture]
public class DatabaseHandlerTests
{
    private string _databasePath;
    private DatabaseHandler _dbHandler;

    [SetUp]
    public async Task SetUp()
    {
        Hashing hash = new Hashing("password");
        await hash.GenerateHashAsync();
        hash.StoreMasterKey(); 
        var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        Console.WriteLine($"Current working directory: {currentDirectory}");
        _databasePath = @"Data/Database.db"; // Path to your existing database

        // Ensure the database file exists
        Assert.That(System.IO.File.Exists(_databasePath), Is.True, "Database file does not exist.");
    }
    
    [Test]
    public void TestCreateWebsiteContent()
    {
        _dbHandler = new DatabaseHandler(_databasePath, "Website1", "https://test","xPath1", 
            "elementId1", "content1", false, null, null);
        // Act
        _dbHandler.CreateWebsiteContent();
        // Assert
        var content = _dbHandler.GetAllRecordsFromTable("website_content");
        var foundRow = content.Select("website_name = 'Website1'").FirstOrDefault();
        Assert.That(foundRow, Is.Not.Null, "Website content should be inserted.");
        Assert.That(foundRow["website_name"], Is.EqualTo("Website1"));
        Assert.That(foundRow["url"], Is.EqualTo("https://test"));
        Assert.That(foundRow["x_path"], Is.EqualTo("xPath1"));
        Assert.That(foundRow["element_id"], Is.EqualTo("elementId1"));
        Assert.That(foundRow["content"], Is.EqualTo("content1"));
        Assert.That(foundRow["login"], Is.EqualTo(0));
    }
    
    [Test]
    public void TestCreateWebsiteWithLogin()
    {
        _dbHandler = new DatabaseHandler(_databasePath, "Website2", "https://test2","xPath2", 
            null, "content2", true, "password2", "password1");
        // Act
        _dbHandler.CreateWebsiteContent();
        // Assert
        var content = _dbHandler.GetAllRecordsFromTable("website_content");
        var foundRow = content.Select("website_name = 'Website2'").FirstOrDefault();
        Assert.That(foundRow, Is.Not.Null, "Website content should be inserted.");
        Assert.That(foundRow["website_name"], Is.EqualTo("Website2"));
        Assert.That(foundRow["url"], Is.EqualTo("https://test2"));
        Assert.That(foundRow["x_path"], Is.EqualTo("xPath2"));
        Assert.That(foundRow["element_id"], Is.EqualTo(DBNull.Value));
        Assert.That(foundRow["content"], Is.EqualTo("content2"));
        Assert.That(foundRow["login"], Is.EqualTo(1));
    }
    
}
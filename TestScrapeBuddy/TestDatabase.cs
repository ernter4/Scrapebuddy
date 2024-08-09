using ScrapeBuddy.Data;

namespace TestScrapeBuddy;

[TestFixture]
public class DatabaseHandlerTests
{
    private string _databasePath;
    private DatabaseHandler _dbHandler;

    [SetUp]
    public void SetUp()
    {
        var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        Console.WriteLine($"Current working directory: {currentDirectory}");
        _databasePath = @"Data/Database.db"; // Path to your existing database
        _dbHandler = new DatabaseHandler(_databasePath, "Website1", "xPath1", 
            "elementId1", "content1", null, null);

        // Ensure the database file exists
        Assert.That(System.IO.File.Exists(_databasePath), Is.True, "Database file does not exist.");
    }
    
    [Test]
    public void TestCreateWebsiteContent()
    {
        // Act
        int websiteId = _dbHandler.CreateWebsiteContent();
        Console.WriteLine(websiteId);
        // Assert
        var content = _dbHandler.GetAllRecordsFromTable("website_content");
        var foundRow = content.Select("website_name = 'Website1'").FirstOrDefault();
        Assert.That(foundRow, Is.Not.Null, "Website content should be inserted.");
        Assert.That(foundRow["website_name"], Is.EqualTo("Website1"));
        Assert.That(foundRow["x_path"], Is.EqualTo("xPath1"));
        Assert.That(foundRow["element_id"], Is.EqualTo("elementId1"));
        Assert.That(foundRow["content"], Is.EqualTo("content1"));
        
        _dbHandler.CreateUser(websiteId);
        
    }
    
}
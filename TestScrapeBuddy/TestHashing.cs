using ScrapeBuddy.Logic;

namespace TestScrapeBuddy;

class TestHashing{
    [Test]
    public async Task TestGenerateHashAsync()
    {
        Hashing hash = new Hashing("password");
        await hash.GenerateHashAsync();
        var info = new FileInfo("Data/ScrapeBuddy.conf");
        if (info.Length != 0){
            Assert.Pass();
        } else {
            Assert.Fail();
        }
    }
    
    [Test]
    public void TestVerify(){
        Hashing hash = new Hashing("password");
        Assert.That(hash.Verify(), Is.EqualTo(true) ); 
    }

    [Test]
    public void TestMasterKey(){
        Hashing hash = new Hashing("password");
        hash.StoreMasterKey();
        string masterKey = Hashing.GetMasterKey();
        Assert.That(masterKey, Is.EqualTo("password"));
        
        Hashing.ClearMasterKey();
        masterKey = Hashing.GetMasterKey();
        Assert.That(masterKey, Is.Null);
    }
}
 

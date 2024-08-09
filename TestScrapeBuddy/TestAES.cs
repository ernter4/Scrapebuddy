
using ScrapeBuddy.Logic;

namespace TestScrapeBuddy;

[TestFixture]
public class AESTest
{
    public byte[] encUsername;
    public byte[] encPassword;
    public byte[] salt;
    public byte[] IV;
    public string key = "key";
    public string username = "username";
    public string password = "password";
    
    [Test]
    public void TestEncryption()
    {
        Encryption enc = new Encryption(username, password, key);
        
        var resultEnc = enc.EncryptStringToBytes_Aes();
        salt = resultEnc.salt;
        IV = resultEnc.IV;
        encUsername = resultEnc.encUsername;
        encPassword = resultEnc.encPassword;
        
        Console.WriteLine($"Encrypted Username: {Convert.ToBase64String(encUsername)} \n");
        Console.WriteLine($"Encrypted Password: {Convert.ToBase64String(encPassword)} \n");
        Console.WriteLine($"Salt: {Convert.ToBase64String(salt)} \n");
        Console.WriteLine($"IV: {Convert.ToBase64String(IV)} \n");
        
        Assert.That(resultEnc.encUsername, Is.Not.EqualTo("username"));
        Assert.That(resultEnc.encPassword, Is.Not.EqualTo("password"));
        
        Decryption dec = new Decryption(encUsername, encPassword, key, salt, IV);
        var resultDec = dec.DecryptStringFromBytes_Aes();
        
        Console.WriteLine($"Username: {resultDec.username} \n"); 
        Console.WriteLine($"Password: {resultDec.password} \n"); 
        Assert.That(resultDec.username, Is.EqualTo("username"));
        Assert.That(resultDec.password, Is.EqualTo("password"));
    }
}






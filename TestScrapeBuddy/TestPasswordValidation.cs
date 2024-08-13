using ScrapeBuddy.Logic;

namespace TestScrapeBuddy;

[TestFixture]
public class PasswordValidationTests
{
    [Test]
    public void TestCreateWebsiteContent()
    {
        var validPassword = "Password1!";
        var shortPassword = "Pwd1!";
        var noUppercasePassword = "password1!";
        var noLowercasePassword = "PASSWORD1!";
        var noDigitPassword = "Password!";
        var noSpecialCharPassword = "Password1";
        
        Assert.That(PasswordValidator.ValidatePassword(validPassword), Is.True);
        Assert.That(PasswordValidator.ValidatePassword(shortPassword), Is.False);
        Assert.That(PasswordValidator.ValidatePassword(noUppercasePassword), Is.False);
        Assert.That(PasswordValidator.ValidatePassword(noLowercasePassword), Is.False);
        Assert.That(PasswordValidator.ValidatePassword(noDigitPassword), Is.False);
        Assert.That(PasswordValidator.ValidatePassword(noSpecialCharPassword), Is.False); 
        
    }
    
}
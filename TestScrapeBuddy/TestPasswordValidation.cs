using ScrapeBuddy.Logic;
using NUnit.Framework;
using System.Collections.Generic;

namespace TestScrapeBuddy
{
    [TestFixture]
    public class PasswordValidationTests
    {
        /// <summary>
        /// Tests various passwords against the validation criteria to ensure they meet the expected results.
        /// </summary>
        [Test]
        public void TestPasswordValidation()
        {
            // Define test cases
            var validPassword = "Password1!";
            var shortPassword = "Pwd1!";
            var noUppercasePassword = "password1!";
            var noLowercasePassword = "PASSWORD1!";
            var noDigitPassword = "Password!";
            var noSpecialCharPassword = "Password1";

            // Define expected results for each test case
            var expectedValidPasswordResults = new List<bool> { true, true, true, true, true };
            var expectedShortPasswordResults = new List<bool> { false, true, true, true, true };
            var expectedNoUppercasePasswordResults = new List<bool> { true, false, true, true, true };
            var expectedNoLowercasePasswordResults = new List<bool> { true, true, false, true, true };
            var expectedNoDigitPasswordResults = new List<bool> { true, true, true, false, true };
            var expectedNoSpecialCharPasswordResults = new List<bool> { true, true, true, true, false };

            // Execute tests
            Assert.That(PasswordValidator.ValidatePassword(validPassword), Is.EqualTo(expectedValidPasswordResults));
            Assert.That(PasswordValidator.ValidatePassword(shortPassword), Is.EqualTo(expectedShortPasswordResults));
            Assert.That(PasswordValidator.ValidatePassword(noUppercasePassword), Is.EqualTo(expectedNoUppercasePasswordResults));
            Assert.That(PasswordValidator.ValidatePassword(noLowercasePassword), Is.EqualTo(expectedNoLowercasePasswordResults));
            Assert.That(PasswordValidator.ValidatePassword(noDigitPassword), Is.EqualTo(expectedNoDigitPasswordResults));
            Assert.That(PasswordValidator.ValidatePassword(noSpecialCharPassword), Is.EqualTo(expectedNoSpecialCharPasswordResults));
        }

        /// <summary>
        /// Tests if the password meets all criteria.
        /// </summary>
        [Test]
        public void TestAreAllCriteriaMet()
        {
            // Define test cases
            var validPassword = "Password1!";
            var invalidPassword = "Password";

            // Execute tests
            Assert.That(PasswordValidator.AreAllCriteriaMet(validPassword), Is.True);
            Assert.That(PasswordValidator.AreAllCriteriaMet(invalidPassword), Is.False);
        }
    }
}

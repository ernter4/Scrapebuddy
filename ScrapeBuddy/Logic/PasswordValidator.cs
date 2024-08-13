using System.Linq;

namespace ScrapeBuddy.Logic
{
    /// <summary>
    /// Provides methods for validating passwords according to specific security rules.
    /// </summary>
    public class PasswordValidator
    {
        /// <summary>
        /// Validates the provided password based on length, presence of uppercase and lowercase letters, digits, and special characters.
        /// </summary>
        /// <param name="password">The password string to be validated.</param>
        /// <returns><c>true</c> if the password meets all validation criteria; otherwise, <c>false</c>.</returns>
        public static bool ValidatePassword(string password)
        {
            // Check if the password is at least 8 characters long
            if (password.Length < 8)
            {
                return false;
            }

            // Check if the password contains at least one uppercase letter
            if (!password.Any(char.IsUpper))
            {
                return false;
            }

            // Check if the password contains at least one lowercase letter
            if (!password.Any(char.IsLower))
            {
                return false;
            }

            // Check if the password contains at least one digit
            if (!password.Any(char.IsDigit))
            {
                return false;
            }

            // Check if the password contains at least one special character from the specified set
            string specialCharacters = "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
            if (!password.Any(c => specialCharacters.Contains(c)))
            {
                return false;
            }

            // If all checks pass, the password is valid
            return true;
        }
    }
}



using System.Collections.Generic;
using System.Linq;

namespace ScrapeBuddy.Logic
{
    /// <summary>
    /// Provides methods for validating passwords according to security rules.
    /// </summary>
    public class PasswordValidator
    {
        private const int MinimumLength = 8;
        private static readonly string SpecialCharacters = "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";

        /// <summary>
        /// Validates the provided password against several criteria.
        /// </summary>
        /// <param name="password">The password to validate.</param>
        /// <returns>A list of boolean values indicating whether each criterion is met.</returns>
        public static List<bool> ValidatePassword(string password)
        {
            // Handle null or empty password
            if (string.IsNullOrEmpty(password))
            {
                return new List<bool>
                {
                    false, // Length validity
                    false, // Uppercase
                    false, // Lowercase
                    false, // Digit
                    false  // Special character
                };
            }

            return new List<bool>
            {
                IsLengthValid(password),
                ContainsUppercase(password),
                ContainsLowercase(password),
                ContainsDigit(password),
                ContainsSpecialCharacter(password)
            };
        }
        
        public static bool AreAllCriteriaMet(string password)
        {
            List<bool> validationResults = ValidatePassword(password);
    
            // Check if all elements in the validationResults list are true
            bool allTrue = validationResults.All(result => result);

            return allTrue;
        }

        /// <summary>Checks if the password length is valid.</summary>
        private static bool IsLengthValid(string password) => password.Length >= MinimumLength;

        /// <summary>Checks if the password contains at least one uppercase letter.</summary>
        private static bool ContainsUppercase(string password) => password.Any(char.IsUpper);

        /// <summary>Checks if the password contains at least one lowercase letter.</summary>
        private static bool ContainsLowercase(string password) => password.Any(char.IsLower);

        /// <summary>Checks if the password contains at least one digit.</summary>
        private static bool ContainsDigit(string password) => password.Any(char.IsDigit);

        /// <summary>Checks if the password contains at least one special character.</summary>
        private static bool ContainsSpecialCharacter(string password) => password.Any(c => SpecialCharacters.Contains(c));
    }
}

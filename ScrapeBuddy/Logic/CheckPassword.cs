using System;
using System.Linq;
using System.Text.RegularExpressions;

public class PasswordValidator
{
    public static bool ValidatePassword(string password)
    {
        // Überprüfung der Länge
        if (password.Length < 8)
        {
            return false;
        }

        // Überprüfung auf Großbuchstaben
        if (!password.Any(char.IsUpper))
        {
            return false;
        }

        // Überprüfung auf Kleinbuchstaben
        if (!password.Any(char.IsLower))
        {
            return false;
        }

        // Überprüfung auf Ziffern
        if (!password.Any(char.IsDigit))
        {
            return false;
        }

        // Überprüfung auf Sonderzeichen
        string specialCharacters = "%$&§/?";
        if (!password.Any(c => specialCharacters.Contains(c)))
        {
            return false;
        }

        // Alle Bedingungen sind erfüllt
        return true;
    }
}


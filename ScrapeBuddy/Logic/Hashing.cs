using System.Threading.Tasks;
using System.IO;
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace ScrapeBuddy.Logic
{
    /// <summary>
    /// The Hashing class provides methods to generate and verify password hashes,
    /// including the use of salting, and securely store and retrieve a master key using SecureString.
    /// </summary>
    public class Hashing
    {
        private string? _passwordHash;
        private readonly string _password;
        private static SecureString? _masterKey;
        private readonly string _salt;

        /// <summary>
        /// Initializes a new instance of the Hashing class with the specified password.
        /// A random salt is generated for additional security.
        /// </summary>
        /// <param name="password">The password to be hashed and used for storing the master key.</param>
        public Hashing(string password)
        {
            this._password = password;
            this._salt = BCrypt.Net.BCrypt.GenerateSalt(); // Generate a new salt
        }

        /// <summary>
        /// Asynchronously generates a hash of the password with the salt and writes it to a configuration file.
        /// The salt is stored alongside the hash.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task GenerateHashAsync()
        {
            string docPath = "Data";
            string filePath = Path.Combine(docPath, "ScrapeBuddy.conf");
            
            string passwordWithSalt = _password + _salt;
            string passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(passwordWithSalt);
            
            Directory.CreateDirectory(docPath);

            try
            {
                // Save both the salt and the hash, separated by a colon
                await File.WriteAllTextAsync(filePath, $"{_salt}:{passwordHash}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while writing the password hash: {ex.Message}");
            }
        }

        /// <summary>
        /// Verifies the password against the stored hash.
        /// </summary>
        /// <returns>True if the password matches the stored hash; otherwise, false.</returns>
        public bool Verify()
        {
            bool verify;
            using StreamReader reader = new("Data/ScrapeBuddy.conf");
            string fileContent = reader.ReadToEnd();

            // Split the stored value into salt and hash
            var parts = fileContent.Split(':');
            if (parts.Length != 2)
            {
                throw new InvalidOperationException("The stored password hash is in an invalid format.");
            }

            string storedSalt = parts[0];
            string storedHash = parts[1];

            string passwordWithStoredSalt = _password + storedSalt;
            verify = BCrypt.Net.BCrypt.EnhancedVerify(passwordWithStoredSalt, storedHash);
            return verify;
        }

        /// <summary>
        /// Stores the password securely as a master key using SecureString.
        /// </summary>
        public void StoreMasterKey()
        {
            _masterKey = new SecureString();
            foreach (char c in this._password)
            {
                _masterKey.AppendChar(c);
            }
            _masterKey.MakeReadOnly();
        }

        /// <summary>
        /// Retrieves the master key as a plain text string.
        /// </summary>
        /// <returns>The master key as a plain text string, or null if the master key is not set.</returns>
        public static string GetMasterKey()
        {
            if (_masterKey == null)
            {
                return null;
            }
            
            IntPtr bstr = IntPtr.Zero;
            try
            {
                bstr = Marshal.SecureStringToBSTR(_masterKey!);
                return Marshal.PtrToStringBSTR(bstr);
            }
            finally
            {
                if (bstr != IntPtr.Zero)
                {
                    Marshal.FreeBSTR(bstr);
                }
            }
        }

        /// <summary>
        /// Clears and disposes of the stored master key.
        /// </summary>
        public static void ClearMasterKey()
        {
            _masterKey?.Dispose();
            _masterKey = null;
        }
    }
}

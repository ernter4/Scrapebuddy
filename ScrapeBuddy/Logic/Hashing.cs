using System.Threading.Tasks;
using System.IO;
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace ScrapeBuddy.Logic
{
    /// <summary>
    /// The Hashing class provides methods to generate and verify password hashes
    /// and securely store and retrieve a master key using SecureString.
    /// </summary>
    public class Hashing
    {
        private string? _passwordHash;
        private string _password;
        private static SecureString? _masterKey;

        /// <summary>
        /// Initializes a new instance of the Hashing class with the specified password.
        /// </summary>
        /// <param name="password">The password to be hashed and used for storing the master key.</param>
        public Hashing(string password)
        {
            this._password = password;
        }

        /// <summary>
        /// Asynchronously generates a hash of the password and writes it to a configuration file.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task GenerateHashAsync()
        {
            string docPath = "Data";
            string filePath = Path.Combine(docPath, "ScrapeBuddy.conf");
            string passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(_password);
            
            Directory.CreateDirectory(docPath);

            try
            {
                await File.WriteAllTextAsync(filePath, passwordHash);
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
            _passwordHash = reader.ReadToEnd();
            verify = BCrypt.Net.BCrypt.EnhancedVerify(_password, _passwordHash);
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

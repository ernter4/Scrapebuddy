using System.Security.Cryptography;
using System.IO;

namespace ScrapeBuddy.Logic;

/// <summary>
/// Provides methods for encrypting data using AES and PBKDF2.
/// </summary>
public class Encryption
{
    /// <summary>Size of the salt in bytes.</summary>
    public const int SALT_SIZE = 24;
    /// <summary>Size of the hash in bytes.</summary>
    public const int HASH_SIZE = 32;
    /// <summary>Number of iterations for PBKDF2.</summary>
    public const int ITERATIONS = 100000;

    private string username;
    private string password;
    private string _key;

    /// <summary>
    /// Initializes a new instance of the Encryption class.
    /// </summary>
    /// <param name="username">The username to encrypt.</param>
    /// <param name="password">The password to encrypt.</param>
    /// <param name="key">The encryption key.</param>
    public Encryption(string username, string password, string key)
    {
        this.username = username;
        this.password = password;
        this._key = key; 
    }

    /// <summary>
    /// Derives a key and salt using PBKDF2.
    /// </summary>
    /// <param name="key">The key to use for derivation.</param>
    /// <returns>A tuple containing the salt and derived key.</returns>
    public (byte[] salt, byte[] Key) KDF(string key)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SALT_SIZE);
        using (var pbkdf2 = new Rfc2898DeriveBytes(key, salt, ITERATIONS, HashAlgorithmName.SHA256))
        {
            return (salt, pbkdf2.GetBytes(HASH_SIZE));
        }    
    }

    /// <summary>
    /// Encrypts the username and password using AES.
    /// </summary>
    /// <returns>A tuple containing the encrypted username, encrypted password, salt, and IV.</returns>
    public (byte[] encUsername, byte[] encPassword, byte[] salt, byte[] IV) EncryptStringToBytes_Aes()
    {
        byte[] encUsername;
        byte[] encPassword;
        byte[] IV;
        var kdf = KDF(_key);

        using (Aes aesAlg = Aes.Create())
        {     
            aesAlg.GenerateIV();
            aesAlg.Key = kdf.Key;
            IV = aesAlg.IV;  
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(username);
                    }
                    encUsername = msEncrypt.ToArray();
                }
            }

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(password);
                    }
                    encPassword = msEncrypt.ToArray();
                }
            }
        }

        return (encUsername, encPassword, kdf.salt, IV);
    }
}

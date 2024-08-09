using System.Security.Cryptography;
using System.IO;

namespace ScrapeBuddy.Logic;

/// <summary>
/// Provides methods for decrypting data using AES and PBKDF2.
/// </summary>
public class Decryption
{
    private byte[] _encUsername;
    private byte[] _encPassword;
    private byte[] _salt;
    private byte[] _IV;
    public const int HASH_SIZE = Encryption.HASH_SIZE;
    public const int ITERATIONS = Encryption.ITERATIONS;
    private string _key;
 
    /// <summary>
    /// Initializes a new instance of the Decryption class.
    /// </summary>
    /// <param name="encUsername">The encrypted username.</param>
    /// <param name="encPassword">The encrypted password.</param>
    /// <param name="key">The encryption key.</param>
    /// <param name="salt">The salt used for key derivation.</param>
    /// <param name="IV">The initialization vector used for encryption.</param>
    public Decryption(byte[] encUsername, byte[] encPassword, string key, byte[] salt, byte[] IV )
    {
        this._encUsername = encUsername;
        this._encPassword = encPassword;
        this._salt = salt;
        this._IV = IV;
        this._key = key;
    }

    /// <summary>
    /// Derives a key using PBKDF2 with the specified salt.
    /// </summary>
    /// <param name="key">The key to use for derivation.</param>
    /// <param name="salt">The salt used for key derivation.</param>
    /// <returns>The derived key.</returns>
    public byte[] KDF(string key, byte[] salt)
    {
        using (var pbkdf2 = new Rfc2898DeriveBytes(key, salt, ITERATIONS, HashAlgorithmName.SHA256))
        {
            return pbkdf2.GetBytes(HASH_SIZE);
        }   
    }

    /// <summary>
    /// Decrypts the username and password using AES.
    /// </summary>
    /// <returns>A tuple containing the decrypted username and password.</returns>
    public (string username, string password) DecryptStringFromBytes_Aes()
    {
        string username;
        string password;

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = KDF(_key, _salt);
            aesAlg.IV = _IV;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(_encUsername))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        username = srDecrypt.ReadToEnd();
                    }
                }
            }

            using (MemoryStream msDecrypt = new MemoryStream(_encPassword))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        password = srDecrypt.ReadToEnd();
                    }
                }
            }
        }

        return (username, password);
    }
}

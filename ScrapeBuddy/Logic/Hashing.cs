using System.Threading.Tasks;
using System.IO;
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace ScrapeBuddy.Logic;

public class Hashing{
    private string? _passwordHash;
    private string _password;
    private static SecureString? _masterKey;

    public Hashing(string password){
        this._password = password;
    }
    
    public async Task GenerateHashAsync(){
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

    public bool Verify(){
        bool verify;
        using StreamReader reader = new("Data/ScrapeBuddy.conf");
        _passwordHash = reader.ReadToEnd();
        verify = BCrypt.Net.BCrypt.EnhancedVerify(_password, _passwordHash);
        return verify;
    }

    public void StoreMasterKey()
    {
        _masterKey = new SecureString();
        foreach (char c in this._password)
        {
            _masterKey.AppendChar(c);
        }
        _masterKey.MakeReadOnly();
    }

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

    public static void ClearMasterKey()
    {
        _masterKey?.Dispose();
        _masterKey = null;
    }
}

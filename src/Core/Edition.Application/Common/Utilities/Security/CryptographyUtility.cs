using System.Text;
using System.Security.Cryptography;

namespace Edition.Application.Common.Utilities.Security;

public static class CryptographyUtility
{
    private const string GeneralHashKey = "BE1272AE-B22E-4779-97D5-172BFCF437C1";

    public static string Encrypt(this string value)
        => Encrypt(value, GeneralHashKey);

    public static string Decrypt(this string value)
        => Decrypt(value, GeneralHashKey);

    public static string Encrypt(this string value, string key)
    {
        byte[] encryptedBytes = [];
        // Set up the encryption objects
        using (Aes aes = Aes.Create())
        {
            var utf8 = new UTF8Encoding();

            aes.Key = utf8.GetBytes(key);
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            
            var input = utf8.GetBytes(value);
            // Encrypt the input plaintext using the AES algorithm
            using ICryptoTransform encryptor = aes.CreateEncryptor();
            encryptedBytes = encryptor.TransformFinalBlock(input, 0, input.Length);
        }
        return Convert.ToBase64String(encryptedBytes)
            .Replace("+", "-1-")
            .Replace("=", "-2-")
            .Replace("/", "-3-")
            .Replace("\\", "-4-");
    }

    public static string Encrypt(this byte[] plainBytes, byte[] key)
    {
        byte[] encryptedBytes = [];
        // Set up the encryption objects
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;

            // Encrypt the input plaintext using the AES algorithm
            using ICryptoTransform encryptor = aes.CreateEncryptor();
            encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        }
        return Convert.ToBase64String(encryptedBytes)
            .Replace("+", "-1-")
            .Replace("=", "-2-")
            .Replace("/", "-3-")
            .Replace("\\", "-4-");
    }

    public static string Decrypt(this string value, string key)
    {
        key = key.Replace("-1-", "+")
            .Replace("-2-", "=")
            .Replace("-3-", "/")
            .Replace("-4-", "\\");

        byte[] decryptedBytes = [];

        var utf8 = new UTF8Encoding();
        // Set up the encryption objects
        using (Aes aes = Aes.Create())
        {           
            aes.Key = utf8.GetBytes(key);
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;

            var input = utf8.GetBytes(value);
            // Decrypt the input ciphertext using the AES algorithm
            using ICryptoTransform decryptor = aes.CreateDecryptor();
            decryptedBytes = decryptor.TransformFinalBlock(input, 0, input.Length);
        }
        return utf8.GetString(decryptedBytes);        
    }

    public static byte[] Decrypt(this byte[] cipherBytes, byte[] key)
    {
        byte[] decryptedBytes = [];

        // Set up the encryption objects
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;

            // Decrypt the input ciphertext using the AES algorithm
            using ICryptoTransform decryptor = aes.CreateDecryptor();
            decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
        }
        return decryptedBytes;
    }
}
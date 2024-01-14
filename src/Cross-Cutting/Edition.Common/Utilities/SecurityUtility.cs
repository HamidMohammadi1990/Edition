using System.Text;
using System.Security.Cryptography;

namespace Edition.Common.Utilities;

public static class SecurityUtility
{
    public static string GetSha256Hash(string input)
    {
        var byteValue = Encoding.UTF8.GetBytes(input);
        var byteHash = SHA256.HashData(byteValue);
        return Convert.ToBase64String(byteHash);
    }
}
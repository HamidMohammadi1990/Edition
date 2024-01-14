using System.Security.Cryptography;

namespace Edition.Common.Utilities;

public static class NumberGenerator
{
    public static int CreateOTPCode()
        => RandomNumberGenerator.GetInt32(10000, 100000);
}
namespace Edition.Common.Models;

public class SiteSettings
{
    public string ElmahPath { get; set; }
    public JwtSettings JwtSettings { get; set; }    
}
public class JwtSettings
{
    public string SecretKey { get; set; }
    public string EncryptKey { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int NotBeforeMinutes { get; set; }
    public int ExpirationMinutes { get; set; }
}
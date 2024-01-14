using System.IdentityModel.Tokens.Jwt;

namespace Edition.Application.Models.Services;

public class AccessTokenResponse(JwtSecurityToken securityToken, string refreshToken)
{
    public string Access_token { get; set; } = new JwtSecurityTokenHandler().WriteToken(securityToken);
    public string Refresh_token { get; set; } = refreshToken;
    public string Token_type { get; set; } = "Bearer";
    public int Expires_in { get; set; } = (int)(securityToken.ValidTo - DateTime.UtcNow).TotalSeconds;
}
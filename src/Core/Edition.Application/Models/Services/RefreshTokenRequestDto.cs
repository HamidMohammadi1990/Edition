namespace Edition.Application.Models.Services;

public class RefreshTokenRequestDto
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}
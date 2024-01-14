namespace Edition.Application.Models.Services;

public class CheckTokenRequestDto(string token)
{
    public string Token { get; set; } = token;
}
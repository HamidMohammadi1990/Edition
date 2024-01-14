namespace Edition.Application.Features.Users.Commands.SignInUser;

public record SignInResponseDto(string Access_token, string Refresh_token, string Token_type, int Expires_in);
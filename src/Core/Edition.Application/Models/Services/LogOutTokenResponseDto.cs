namespace Edition.Application.Models.Services;

public class LogOutTokenResponseDto(bool loggedOut)
{
    public bool LoggedOut { get; set; } = loggedOut;
}
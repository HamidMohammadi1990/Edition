namespace Edition.Application.Features.Users.Queries.UserNameCheck;

public record UserNameCheckRequestQuery(string UserName) : IRequest<UserNameCheckResponseDto>;
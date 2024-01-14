using Edition.Common.Models;

namespace Edition.Application.Features.Users.Commands.SignInUser;

public record SignInUserCommand(string UserName, string Password)
    : IRequest<OperationResult<SignInResponseDto>>;
using Edition.Common.Models;

namespace Edition.Application.Features.Users.Commands.RegisterUser;

public record RegisterUserCommand(string UserName, string OTPCode)
    : IRequest<OperationResult<bool>>;
using Edition.Common.Models;

namespace Edition.Application.Features.Users.Commands.SignOutUser;

public record SignOutUserCommand(string Token) : IRequest<OperationResult<bool>>;
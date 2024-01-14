using Edition.Common.Models;
using Edition.Application.Services.Contracts;

namespace Edition.Application.Features.Users.Commands.SignOutUser;

public class SignOutUserCommandHandler
    (IAccountingService accountingService)
    : IRequestHandler<SignOutUserCommand, OperationResult<bool>>
{
    public async Task<OperationResult<bool>> Handle(SignOutUserCommand request, CancellationToken cancellationToken)
    {
        var blockTokenResult = await accountingService.BlockTokenAsync(new(request.Token));
        if (!blockTokenResult.Result!.LoggedOut)
            return OperationResult<bool>.Fail();

        return OperationResult<bool>.Success(true);
    }
}
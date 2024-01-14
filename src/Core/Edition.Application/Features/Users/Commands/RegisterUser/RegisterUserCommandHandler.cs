using Edition.Common.Models;
using Edition.Domain.Entities;
using Edition.Application.Common.Contracts;

namespace Edition.Application.Features.Users.Commands.RegisterUser;

public class RegisterUserCommandHandler
    (IEditionContext context)
    : IRequestHandler<RegisterUserCommand, OperationResult<bool>>
{
    public async Task<OperationResult<bool>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // TODO Validate OTP Code
        var isValidOTPCode = true;
        if (!isValidOTPCode)
            return OperationResult<bool>.Fail("کد یکبار مصرف معتبر نمی باشد");

        var user = new User
        {
            UserName = request.UserName,
            PhoneNumber = request.UserName,
            IsActive = true,
            PhoneNumberConfirmed = true
        };
        context.User.Add(user);
        var saveChangesResult = await context.SaveAllChangesAsync(cancellationToken);
        if (!saveChangesResult.IsSuccess)
            return OperationResult<bool>.Fail();

        return OperationResult<bool>.Success(true);
    }
}
using Edition.Common.Models;
using Edition.Domain.Entities;
using Edition.Common.Utilities;
using Edition.Application.Common.Contracts;
using Edition.Application.Services.Contracts;

namespace Edition.Application.Features.Users.Commands.SignInUser;

public class SignInUserCommandHandler
    (IEditionContext context, IAccountingService accountingService)
    : IRequestHandler<SignInUserCommand, OperationResult<SignInResponseDto>>
{
    public async Task<OperationResult<SignInResponseDto>> Handle(SignInUserCommand request, CancellationToken cancellationToken)
    {
        var passwordHash = SecurityUtility.GetSha256Hash(request.Password);
        var user = await context
                         .User
                         .SingleOrDefaultAsync(x => x.UserName == request.UserName ||
                                               x.Email == request.UserName &&
                                               x.PasswordHash == passwordHash, cancellationToken);

        if (user is null)
            return OperationResult<SignInResponseDto>
                   .Fail("نام کاربری یا رمز عبور صحیح نمی باشد.");

        var tokenResponse = await accountingService.GenerateTokenAsync(user);
        var refreshToken = new RefreshToken(user.Id, tokenResponse.Result!.Refresh_token, DateTime.Now);
        context.RefreshToken.Add(refreshToken);
        var saveChangesResult = await context.SaveAllChangesAsync(cancellationToken);
        if (!saveChangesResult.IsSuccess)
            return OperationResult<SignInResponseDto>.Fail();

        var result = new SignInResponseDto(
                        tokenResponse.Result.Access_token,
                        tokenResponse.Result.Refresh_token,
                        tokenResponse.Result.Token_type,
                        tokenResponse.Result.Expires_in);
        return OperationResult<SignInResponseDto>.Success(result);
    }
}
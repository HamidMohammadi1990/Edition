using Edition.Common.Models;
using Edition.Domain.Entities;
using Edition.Application.Common.Contracts;

namespace Edition.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler
    (IEditionContext context)
    : IRequestHandler<CreateUserCommand, OperationResult<int>>
{
    public async Task<OperationResult<int>> Handle(CreateUserCommand request, CancellationToken cancellationToken = default)
    {
        var user = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            PasswordHash = request.Password,
            FirstName = request.FirstName,
            LastName = request.LastName,
            IsActive = true,
            Gender = request.Gender
        };
        context.User.Add(user);
        var saveChangesResult = await context.SaveAllChangesAsync(cancellationToken);
        if (!saveChangesResult.IsSuccess)
            return OperationResult<int>.Fail();

        return OperationResult<int>.Success(user.Id);
    }
}
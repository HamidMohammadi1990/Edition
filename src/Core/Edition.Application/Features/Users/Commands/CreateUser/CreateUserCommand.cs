using Edition.Domain.Enums;
using Edition.Common.Models;

namespace Edition.Application.Features.Users.Commands.CreateUser;

public record CreateUserCommand : IRequest<OperationResult<int>>
{
    public string UserName { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Password { get; set; } = null!;
    public Gender Gender { get; set; }
}
using FluentValidation;

namespace Edition.Application.Features.Users.Commands.SignOutUser;

public class SignOutUserCommandValidator : AbstractValidator<SignOutUserCommand>
{
    public SignOutUserCommandValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty()
            .NotNull()
            .Must(x => x.Length > 0)
            .WithMessage("درخواست نامعتبر!");
    }
}
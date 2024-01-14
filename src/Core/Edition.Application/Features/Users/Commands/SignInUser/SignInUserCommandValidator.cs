using FluentValidation;
using Edition.Common.Extensions;

namespace Edition.Application.Features.Users.Commands.SignInUser;

public class SignInUserCommandValidator : AbstractValidator<SignInUserCommand>
{
    public SignInUserCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotNull()
            .Must(x => x.Length > 0)
            .WithMessage("نام کاربری معتبر نمی باشد")
            .Must(x => x.IsEmail() || x.IsMobile())
            .WithMessage("لطفا نام کاربری را شماره همراه یا ایمیل وارد کنید");

        RuleFor(x => x.Password)
            .NotNull()
            .Must(x => x.Length > 0)
            .WithMessage("رمز عبور معتبر نمی باشد");
    }
}
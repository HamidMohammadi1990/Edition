using FluentValidation;
using Edition.Common.Extensions;

namespace Edition.Application.Features.Users.Commands.RegisterUser;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotNull()
            .WithMessage("نام کاربری الزامی می باشد")
            .Must(x => x.IsMobile())
            .WithMessage("لطفا شماره همراه وارد کنید");

        RuleFor(x => x.OTPCode)
            .NotNull()
            .NotEmpty()
            .Must(x => x.Length > 0)
            .WithMessage("رمز یکبار مصرف الزامی می باشد");
    }
}
using FluentValidation;
using Edition.Common.Extensions;

namespace Edition.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(u => u.UserName)
               .NotNull()
               .WithMessage("نام کاربری الزامی می باشد")
               .Must(u => u.IsMobile())
               .WithMessage("نام کاربری بایستی شماره همراه وارد گردد")
               .MaximumLength(11)
               .WithMessage("نام کاربری بایستی حداکثر 11 کاراکتر باشد");

        RuleFor(u => u.FirstName)
               .NotNull()
               .WithMessage("نام الزامی می باشد")
               .MaximumLength(50)
               .WithMessage("نام بایستی حداکثر 50 کاراکتر باشد");

        RuleFor(u => u.LastName)
               .NotNull()
               .WithMessage("نام خانوادگی الزامی می باشد")
               .MaximumLength(50)
               .WithMessage("نام خانوادگی بایستی حداکثر 50 کاراکتر باشد");

        RuleFor(u => u.Email)
            .NotNull()
            .WithMessage("ایمیل الزامی می باشد")
            .Must(x => x.IsEmail())
            .WithMessage("ایمیل نامعتبر می باشد");

        RuleFor(u => u.PhoneNumber)
            .Must(x => x.IsMobile())
            .WithMessage("شماره همراه نامعتبر می باشد");

        RuleFor(u => u.Password)
           .NotNull()
           .WithMessage("رمز عبور الزامی می باشد");

        RuleFor(u => u.Gender)
          .IsInEnum()
          .NotNull()
          .WithMessage("جنسیت الزامی می باشد");
    }
}
using FluentValidation;
using Edition.Common.Extensions;

namespace Edition.Application.Features.Users.Queries.UserNameCheck;

public class UserNameCheckQueryValidator : AbstractValidator<UserNameCheckRequestQuery>
{
    public UserNameCheckQueryValidator()
    {
        RuleFor(x => x.UserName)
               .Must(x => x.IsEmail() || x.IsMobile())
               .WithMessage("لطفا شماره همراه یا ایمیل وارد کنید");
    }
}
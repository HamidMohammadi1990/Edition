using FluentValidation;

namespace Edition.Application.Features.Users.Queries.GetUserInfo;

public class GetUserInfoQueryValidator : AbstractValidator<GetUserInfoRequestQuery>
{
    public GetUserInfoQueryValidator()
    {
        RuleFor(u => u.Id)
            .NotEmpty()
            .NotEqual(0)
            .WithMessage("Invalid id");
    }
}
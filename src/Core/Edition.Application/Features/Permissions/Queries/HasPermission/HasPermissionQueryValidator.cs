using FluentValidation;

namespace Edition.Application.Features.Permissions.Queries.HasPermission;

public class HasPermissionQueryValidator : AbstractValidator<HasPermissionRequestQuery>
{
    public HasPermissionQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotNull()
            .NotEqual(0)
            .WithMessage("!کاربر نامعتبر");

        RuleFor(x => x.PermissionType)
            .NotNull()
            .IsInEnum()
            .WithMessage("دسترسی نامعتبر");
    }
}
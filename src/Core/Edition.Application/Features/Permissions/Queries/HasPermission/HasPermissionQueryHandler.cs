using Edition.Domain.Attributes;
using Edition.Domain.Caching.Enums;
using Edition.Application.Common.Contracts;

namespace Edition.Application.Features.Permissions.Queries.HasPermission;

public class HasPermissionQueryHandler
    (IEditionContext context)
    : IRequestHandler<HasPermissionRequestQuery, HasPermissionResponseDto>
{
    [Cache(5, CacheInstanceType.Default)]
    public async Task<HasPermissionResponseDto> Handle(HasPermissionRequestQuery request, CancellationToken cancellationToken)
    {
        var hasPermission =
            await (from UserRole in context.UserRole
                   join RolePermission in context.RolePermission
                   on UserRole.RoleId equals RolePermission.RoleId
                   where UserRole.UserId == request.UserId
                   && RolePermission.PermissionId == request.PermissionType
                   select RolePermission.PermissionId)
                   .AnyAsync(cancellationToken);

        return new HasPermissionResponseDto(hasPermission);
    }
}
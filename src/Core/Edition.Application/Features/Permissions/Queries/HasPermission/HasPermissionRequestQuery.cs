using Edition.Domain.Enums;

namespace Edition.Application.Features.Permissions.Queries.HasPermission;

public record HasPermissionRequestQuery(int UserId, PermissionType PermissionType) : IRequest<HasPermissionResponseDto>;
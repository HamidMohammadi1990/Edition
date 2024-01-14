using Edition.Common.Models;
using Edition.Domain.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Edition.Application.Common.Contracts;

public interface IEditionContext
{
    DbSet<User> User { get; }
    DbSet<Role> Role { get; }
    DbSet<UserRole> UserRole { get; }
    DbSet<Permission> Permission { get; }
    DbSet<RolePermission> RolePermission { get; }
    DbSet<RefreshToken> RefreshToken { get; }


    ChangeTracker ChangeTracker { get; }


    /// <summary>
    /// Save all entities in to database.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<OperationResult> SaveAllChangesAsync(CancellationToken cancellationToken = default);
}
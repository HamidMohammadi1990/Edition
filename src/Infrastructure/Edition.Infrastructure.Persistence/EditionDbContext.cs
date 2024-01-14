using System.Reflection;
using Edition.Common.Models;
using Edition.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Edition.Application.Common.Contracts;
using Edition.Infrastructure.Persistence.Interceptors;

namespace Edition.Infrastructure.Persistence;

public sealed class EditionDbContext
    (DbContextOptions<EditionDbContext> options, ILogger<EditionDbContext> logger)
    : DbContext(options), IEditionContext
{
    public DbSet<User> User { get; set; }
    public DbSet<Role> Role { get; set; }
    public DbSet<UserRole> UserRole { get; set; }
    public DbSet<Permission> Permission { get; set; }
    public DbSet<RolePermission> RolePermission { get; set; }
    public DbSet<RefreshToken> RefreshToken { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(new CleanStringPropertyInterceptor());
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    public async Task<OperationResult> SaveAllChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            return OperationResult.Success();
        }
        catch (Exception exception)
        {
            logger.LogCritical(exception, exception.Message);
            return OperationResult.Fail(exception.Message);
        }
    }
}
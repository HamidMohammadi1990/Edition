using Edition.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edition.Infrastructure.Persistence.Configuration;

public class RolePermissionConfig : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermissions");

        builder.HasOne(p => p.Permission)
               .WithMany(p => p.RolePermissions)
               .HasForeignKey(p => p.PermissionId);

        builder.HasOne(p => p.Role)
               .WithMany(p => p.RolePermissions)
               .HasForeignKey(p => p.RoleId);

        builder.HasIndex(x => x.RoleId);
        builder.HasIndex(x => x.PermissionId);
    }
}
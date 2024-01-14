using Edition.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edition.Infrastructure.Persistence.Configuration;

public class RoleConfig : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");
        builder.Property(p => p.Title).HasMaxLength(50);

        builder.HasMany(x => x.RolePermissions)
               .WithOne(x => x.Role)
               .HasForeignKey(x => x.RoleId);

        builder.HasMany(x => x.UserRoles)
              .WithOne(x => x.Role)
              .HasForeignKey(x => x.RoleId);

        builder.HasIndex(x => x.Title)
               .IsUnique();

        builder.HasQueryFilter(x => x.IsActive);
    }
}
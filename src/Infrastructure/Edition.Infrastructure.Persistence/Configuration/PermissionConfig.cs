using Edition.Domain.Enums;
using Edition.Domain.Entities;
using Edition.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edition.Infrastructure.Persistence.Configuration;

public class PermissionConfig : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");

        builder.Property(x => x.Id)
               .ValueGeneratedNever();

        builder.Property(p => p.Title).HasMaxLength(100);
        builder.Property(p => p.NameSpace).HasMaxLength(250);

        builder.HasOne<Permission>()
               .WithMany(x => x.Parents)
               .HasForeignKey(x => x.ParentId);

        builder.HasMany(x => x.RolePermissions)
               .WithOne(x => x.Permission)
               .HasForeignKey(x => x.PermissionId);

        builder.HasQueryFilter(x => x.IsActive);

        builder.HasData(new Permission
        {
            Id = PermissionType.Product,
            Url = "",
            Title = PermissionType.Product.ToDisplay(),
            IsActive = true,
            PriorityId = 0,
            LevelTypeId = PermissionLevelType.Product,
        });
    }
}
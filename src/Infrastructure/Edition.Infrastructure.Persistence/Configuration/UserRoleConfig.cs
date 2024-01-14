using Edition.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edition.Infrastructure.Persistence.Configuration;

public class UserRoleConfig : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles");

        builder.HasOne(x => x.User)
               .WithMany(x => x.UserRoles)
               .HasForeignKey(x => x.UserId);

        builder.HasOne(x => x.Role)
               .WithMany(x => x.UserRoles)
               .HasForeignKey(x => x.RoleId);

        builder.HasIndex(x => x.UserId);        
    }
}
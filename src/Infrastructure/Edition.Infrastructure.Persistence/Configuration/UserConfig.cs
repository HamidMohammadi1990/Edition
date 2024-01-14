using Edition.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edition.Infrastructure.Persistence.Configuration;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasIndex(x => x.UserName)
               .IsUnique();

        builder.HasIndex(x => x.Email)
              .IsUnique();

        builder.HasIndex(x => x.SecurityStamp)
              .IsUnique();

        builder.Property(u => u.FirstName)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(u => u.LastName)
               .HasMaxLength(80)
               .IsRequired();

        builder.Property(u => u.UserName)
               .HasMaxLength(80)
               .IsRequired();

        builder.Property(u => u.PasswordHash)
               .HasMaxLength(150)
               .IsRequired();

        builder.HasMany(x => x.UserRoles)
               .WithOne(x => x.User)
               .HasForeignKey(x => x.UserId);

        builder.HasQueryFilter(x => x.IsActive);
    }
}
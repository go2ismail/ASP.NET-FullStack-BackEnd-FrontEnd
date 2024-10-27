// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Domain.Constants;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.SecurityManagers.AspNetIdentity;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.FirstName)
            .HasMaxLength(NameConsts.MaxLength)
            .IsRequired();

        builder.Property(u => u.LastName)
            .HasMaxLength(NameConsts.MaxLength)
            .IsRequired();

        builder.Property(u => u.ProfilePictureName)
            .HasMaxLength(NameConsts.MaxLength);

        builder.Property(u => u.IsDeleted)
            .IsRequired();

        builder.Property(u => u.CreatedAt)
            .IsRequired(false);

        builder.Property(u => u.CreatedById)
            .HasMaxLength(UserIdConsts.MaxLength);

        builder.Property(u => u.UpdatedAt)
            .IsRequired(false);

        builder.Property(u => u.UpdatedById)
            .HasMaxLength(UserIdConsts.MaxLength);

        builder.HasIndex(u => u.IsDeleted);
    }
}

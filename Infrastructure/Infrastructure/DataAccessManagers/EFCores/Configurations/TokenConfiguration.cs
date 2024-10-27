// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Domain.Constants;
using Domain.Entities;

using Infrastructure.DataAccessManagers.EFCores.Configurations.Bases;
using Infrastructure.SecurityManagers.AspNetIdentity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataAccessManagers.EFCores.Configurations;

public class TokenConfiguration : BaseEntityConfiguration<Token>
{
    public override void Configure(EntityTypeBuilder<Token> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.UserId)
            .HasMaxLength(UserIdConsts.MaxLength)
            .IsRequired();

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(e => e.RefreshToken)
            .HasMaxLength(LengthConsts.M)
            .IsRequired();

        builder.HasIndex(e => e.UserId).HasDatabaseName("IX_Token_UserId");
        builder.HasIndex(e => e.RefreshToken).HasDatabaseName("IX_Token_RefreshToken");
    }
}

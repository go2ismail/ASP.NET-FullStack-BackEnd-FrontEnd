// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Domain.Constants;
using Domain.Entities;

using Infrastructure.DataAccessManagers.EFCores.Configurations.Bases;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataAccessManagers.EFCores.Configurations;

public class VendorSubGroupConfiguration : BaseEntityCommonConfiguration<VendorSubGroup>
{
    public override void Configure(EntityTypeBuilder<VendorSubGroup> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.VendorGroupId)
            .HasMaxLength(IdConsts.MaxLength)
            .IsRequired();

        builder.HasOne<VendorGroup>()
            .WithMany(x => x.VendorSubGroups)
            .HasForeignKey(x => x.VendorGroupId)
            .OnDelete(DeleteBehavior.NoAction);

    }
}

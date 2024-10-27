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

public class VendorConfiguration : BaseEntityAdvanceConfiguration<Vendor>
{
    public override void Configure(EntityTypeBuilder<Vendor> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.VendorGroupId)
            .HasMaxLength(IdConsts.MaxLength)
            .IsRequired();

        builder.HasOne<VendorGroup>()
            .WithMany()
            .HasForeignKey(x => x.VendorGroupId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(e => e.VendorSubGroupId)
            .HasMaxLength(IdConsts.MaxLength)
            .IsRequired(false);

        builder.HasOne<VendorSubGroup>()
            .WithMany()
            .HasForeignKey(x => x.VendorSubGroupId)
            .OnDelete(DeleteBehavior.NoAction);


        builder.Property(x => x.Street)
            .IsRequired()
            .HasMaxLength(NameConsts.MaxLength);


        builder.Property(x => x.City)
            .IsRequired()
            .HasMaxLength(NameConsts.MaxLength);


        builder.Property(x => x.StateOrProvince)
            .IsRequired()
            .HasMaxLength(NameConsts.MaxLength);


        builder.Property(x => x.ZipCode)
            .IsRequired()
            .HasMaxLength(CodeConsts.MaxLength);


        builder.Property(x => x.Country)
            .IsRequired(false)
            .HasMaxLength(NameConsts.MaxLength);


        builder.Property(x => x.Phone)
            .IsRequired()
            .HasMaxLength(CodeConsts.MaxLength);


        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(NameConsts.MaxLength);


        builder.Property(x => x.Website)
            .IsRequired(false)
            .HasMaxLength(NameConsts.MaxLength);

        builder.HasIndex(e => e.Street).HasDatabaseName("IX_Vendor_Street");
        builder.HasIndex(e => e.City).HasDatabaseName("IX_Vendor_City");
    }
}

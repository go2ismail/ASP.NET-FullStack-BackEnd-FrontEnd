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

public class VendorContactConfiguration : BaseEntityAuditConfiguration<VendorContact>
{
    public override void Configure(EntityTypeBuilder<VendorContact> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.GenderId)
            .HasMaxLength(IdConsts.MaxLength)
            .IsRequired();

        builder.HasOne<Gender>()
            .WithMany()
            .HasForeignKey(x => x.GenderId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(e => e.FirstName)
            .IsRequired()
            .HasMaxLength(NameConsts.MaxLength);

        builder.Property(e => e.LastName)
            .IsRequired()
            .HasMaxLength(NameConsts.MaxLength);

        builder.Property(e => e.JobTitle)
            .IsRequired()
            .HasMaxLength(NameConsts.MaxLength);

        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(EmailConsts.MaxLength);

        builder.Property(e => e.MobilePhone)
            .HasMaxLength(CodeConsts.MaxLength);

        builder.Property(e => e.SocialMedia)
            .HasMaxLength(NameConsts.MaxLength);

        builder.Property(e => e.Address)
            .HasMaxLength(NameConsts.MaxLength);

        builder.Property(e => e.City)
            .HasMaxLength(NameConsts.MaxLength);

        builder.Property(e => e.StateOrProvince)
            .HasMaxLength(NameConsts.MaxLength);

        builder.Property(e => e.ZipCode)
            .HasMaxLength(CodeConsts.MaxLength);

        builder.Property(e => e.Country)
            .HasMaxLength(NameConsts.MaxLength);

        builder.Property(e => e.Phone)
            .IsRequired()
            .HasMaxLength(CodeConsts.MaxLength);

        builder.Property(e => e.Website)
            .HasMaxLength(NameConsts.MaxLength);

        builder.Property(e => e.VendorId)
            .HasMaxLength(IdConsts.MaxLength)
            .IsRequired();

        builder.HasOne<Vendor>()
            .WithMany(x => x.VendorContacts)
            .HasForeignKey(x => x.VendorId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(e => e.FirstName).HasDatabaseName("IX_VendorContact_FirstName");
        builder.HasIndex(e => e.LastName).HasDatabaseName("IX_VendorContact_LastName");
        builder.HasIndex(e => e.JobTitle).HasDatabaseName("IX_VendorContact_JobTitle");
        builder.HasIndex(e => e.Email).HasDatabaseName("IX_VendorContact_Email");
    }
}

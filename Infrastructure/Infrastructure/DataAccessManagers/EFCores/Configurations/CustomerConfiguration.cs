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

public class CustomerConfiguration : BaseEntityAdvanceConfiguration<Customer>
{
    public override void Configure(EntityTypeBuilder<Customer> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.CustomerGroupId)
            .HasMaxLength(IdConsts.MaxLength)
            .IsRequired();

        builder.HasOne<CustomerGroup>()
            .WithMany()
            .HasForeignKey(x => x.CustomerGroupId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(e => e.CustomerSubGroupId)
            .HasMaxLength(IdConsts.MaxLength)
            .IsRequired(false);

        builder.HasOne<CustomerSubGroup>()
            .WithMany()
            .HasForeignKey(x => x.CustomerSubGroupId)
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


        builder.HasIndex(e => e.Street).HasDatabaseName("IX_Customer_Street");
        builder.HasIndex(e => e.City).HasDatabaseName("IX_Customer_City");
    }
}

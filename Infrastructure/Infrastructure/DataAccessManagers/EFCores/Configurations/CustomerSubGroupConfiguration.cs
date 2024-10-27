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

public class CustomerSubGroupConfiguration : BaseEntityCommonConfiguration<CustomerSubGroup>
{
    public override void Configure(EntityTypeBuilder<CustomerSubGroup> builder)
    {
        base.Configure(builder);


        builder.Property(e => e.CustomerGroupId)
            .HasMaxLength(IdConsts.MaxLength)
            .IsRequired();

        builder.HasOne<CustomerGroup>()
            .WithMany(x => x.CustomerSubGroups)
            .HasForeignKey(x => x.CustomerGroupId)
            .OnDelete(DeleteBehavior.NoAction);

    }
}

// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Domain.Entities;

using Infrastructure.DataAccessManagers.EFCores.Configurations.Bases;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataAccessManagers.EFCores.Configurations;

public class CustomerGroupConfiguration : BaseEntityCommonConfiguration<CustomerGroup>
{
    public override void Configure(EntityTypeBuilder<CustomerGroup> builder)
    {
        base.Configure(builder);
    }
}

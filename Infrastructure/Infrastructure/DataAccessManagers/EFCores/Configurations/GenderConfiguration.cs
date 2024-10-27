// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Domain.Entities;

using Infrastructure.DataAccessManagers.EFCores.Configurations.Bases;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataAccessManagers.EFCores.Configurations;

public class GenderConfiguration : BaseEntityCommonConfiguration<Gender>
{
    public override void Configure(EntityTypeBuilder<Gender> builder)
    {
        base.Configure(builder);
    }
}

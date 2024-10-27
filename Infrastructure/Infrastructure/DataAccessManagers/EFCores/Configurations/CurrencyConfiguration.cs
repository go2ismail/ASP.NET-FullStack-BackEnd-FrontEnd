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

public class CurrencyConfiguration : BaseEntityCommonConfiguration<Currency>
{
    public override void Configure(EntityTypeBuilder<Currency> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Symbol)
            .IsRequired()
            .HasMaxLength(CodeConsts.MaxLength);


        builder.HasIndex(e => e.Symbol).HasDatabaseName("IX_Curency_Symbol");
    }
}

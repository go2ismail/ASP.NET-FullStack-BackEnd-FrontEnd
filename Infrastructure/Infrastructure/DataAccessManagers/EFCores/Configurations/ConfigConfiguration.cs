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

public class ConfigConfiguration : BaseEntityCommonConfiguration<Config>
{
    public override void Configure(EntityTypeBuilder<Config> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.CurrencyId)
            .HasMaxLength(IdConsts.MaxLength)
            .IsRequired();

        builder.HasOne<Currency>()
            .WithMany()
            .HasForeignKey(x => x.CurrencyId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(x => x.SmtpHost)
            .IsRequired()
            .HasMaxLength(NameConsts.MaxLength);

        builder.Property(x => x.SmtpPort)
            .IsRequired();

        builder.Property(x => x.SmtpUserName)
            .IsRequired()
            .HasMaxLength(NameConsts.MaxLength);

        builder.Property(x => x.SmtpPassword)
            .IsRequired()
            .HasMaxLength(PasswordConsts.MaxLength);

        builder.Property(x => x.SmtpUseSSL)
            .IsRequired();

        builder.Property(x => x.Active)
            .IsRequired();


        builder.HasIndex(e => e.SmtpHost).HasDatabaseName("IX_Config_SmtpHost");
        builder.HasIndex(e => e.SmtpUserName).HasDatabaseName("IX_Config_SmtpUserName");

    }
}

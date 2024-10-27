// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Domain.Constants;
using Domain.Entities;

namespace Infrastructure.DataAccessManagers.EFCores.Configurations;

using Infrastructure.DataAccessManagers.EFCores.Configurations.Bases;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class FileImageConfiguration : BaseEntityCommonConfiguration<FileImage>
{
    public override void Configure(EntityTypeBuilder<FileImage> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.OriginalName)
            .HasMaxLength(NameConsts.MaxLength)
            .IsRequired(false);

        builder.Property(e => e.GeneratedName)
            .HasMaxLength(NameConsts.MaxLength)
            .IsRequired();

        builder.Property(e => e.Extension)
            .HasMaxLength(CodeConsts.MaxLength);

        builder.Property(e => e.FileSize)
            .IsRequired();

        builder.HasIndex(e => e.OriginalName).HasDatabaseName("IX_FileImage_OriginalName");
        builder.HasIndex(e => e.GeneratedName).HasDatabaseName("IX_FileImage_GeneratedName");
    }
}

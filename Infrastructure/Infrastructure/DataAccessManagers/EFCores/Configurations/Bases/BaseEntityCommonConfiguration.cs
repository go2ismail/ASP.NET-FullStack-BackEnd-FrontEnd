// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Domain.Bases;
using Domain.Constants;

using Infrastructure.SecurityManagers.AspNetIdentity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataAccessManagers.EFCores.Configurations.Bases;

public abstract class BaseEntityCommonConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntityCommon
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        //BaseEntity
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasMaxLength(IdConsts.MaxLength).IsRequired();

        //BaseEntityAudit
        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired(false);

        builder.Property(e => e.CreatedById)
            .IsRequired(false)
            .HasMaxLength(UserIdConsts.MaxLength);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(e => e.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(e => e.UpdatedAt)
            .IsRequired(false);

        builder.Property(e => e.UpdatedById)
            .HasMaxLength(UserIdConsts.MaxLength)
            .IsRequired(false);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(e => e.UpdatedById)
            .OnDelete(DeleteBehavior.Restrict);

        //BaseEntityCommon
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(NameConsts.MaxLength);

        builder.Property(e => e.Description)
            .HasMaxLength(DescriptionConsts.MaxLength);

        builder.HasIndex(e => e.Name)
            .HasDatabaseName($"IX_{typeof(T).Name}_Name");
    }
}

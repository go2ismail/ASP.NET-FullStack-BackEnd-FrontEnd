// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Domain.Bases;
using Domain.Constants;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataAccessManagers.EFCores.Configurations.Bases;

public abstract class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        //BaseEntity
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasMaxLength(IdConsts.MaxLength).IsRequired();
    }
}

// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Domain.Interfaces;

namespace Domain.Bases;

/// <summary>
/// Base class for entities that include audit information such as creation, update, deletion status, and tenant association.
/// Inherits from <see cref="BaseEntity"/> and implements <see cref="IHasIsDeleted"/> 
/// </summary>
public abstract class BaseEntityAudit : BaseEntity, IHasIsDeleted
{
    public bool IsDeleted { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? CreatedById { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedById { get; set; }

    protected BaseEntityAudit() { } // for EF Core
    protected BaseEntityAudit(string? userId)
    {
        IsDeleted = false;
        CreatedAt = DateTime.UtcNow;
        CreatedById = userId?.Trim();
    }

    public BaseEntityAudit SetAsDeleted()
    {
        IsDeleted = true;
        return this;
    }

    public BaseEntityAudit SetAudit(string? userId)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedById = userId?.Trim();
        return this;
    }
}

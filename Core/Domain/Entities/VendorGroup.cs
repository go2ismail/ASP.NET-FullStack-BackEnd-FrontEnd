// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Domain.Bases;
using Domain.Constants;
using Domain.Exceptions;
using Domain.Interfaces;

using System.Collections.ObjectModel;

namespace Domain.Entities;

public class VendorGroup : BaseEntityCommon, IAggregateRoot
{
    public ICollection<VendorSubGroup> VendorSubGroups { get; set; } = new Collection<VendorSubGroup>();

    public VendorGroup() : base() { } //for EF Core
    public VendorGroup(
        string? userId,
        string name,
        string? description
        ) : base(userId, name, description)
    {
        VendorSubGroups = new Collection<VendorSubGroup>();
    }


    public void Update(
        string? userId,
        string name,
        string? description
        )
    {
        Name = name.Trim();
        Description = description?.Trim();

        SetAudit(userId);
    }

    public void Delete(
        string? userId
        )
    {
        foreach (VendorSubGroup vendorSub in VendorSubGroups)
        {
            DeleteSubGroup(userId, vendorSub.Id);
        }
        SetAsDeleted();
        SetAudit(userId);
    }


    public VendorSubGroup CreateSubGroup(
        string? userId,
        string name,
        string? description
        )
    {
        if (VendorSubGroups.Any(x => string.Equals(x.Name.Trim(), name.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new DomainException($"{ExceptionConsts.EntitiyAlreadyExists}: {name}");
        }


        var vendorSubGroup = new VendorSubGroup(userId, this.Id, name, description);
        VendorSubGroups.Add(vendorSubGroup);

        return vendorSubGroup;
    }

    public VendorSubGroup UpdateSubGroup(
        string? userId,
        string vendorSubGroupId,
        string name,
        string? description
    )
    {
        var vendorSubGroup = VendorSubGroups.SingleOrDefault(x => x.Id == vendorSubGroupId.Trim());
        if (vendorSubGroup == null)
        {
            throw new DomainException($"{ExceptionConsts.EntitiyNotFound}");
        }

        if (VendorSubGroups.Any(x => string.Equals(x.Name.Trim(), name.Trim(), StringComparison.OrdinalIgnoreCase)
            && x.Id != vendorSubGroupId.Trim()))
        {
            throw new DomainException($"{ExceptionConsts.EntitiyAlreadyExists}: {name}");
        }


        vendorSubGroup.Update(userId, name, description);

        return vendorSubGroup;
    }


    public VendorSubGroup DeleteSubGroup(string? userId, string vendorSubGroupId)
    {
        var vendorSubGroup = VendorSubGroups.SingleOrDefault(x => x.Id == vendorSubGroupId.Trim());
        if (vendorSubGroup == null)
        {
            throw new DomainException($"{ExceptionConsts.EntitiyNotFound}");
        }

        vendorSubGroup.Delete(userId);

        return vendorSubGroup;
    }
    public VendorSubGroup GetSubGroup(string vendorSubGroupId)
    {
        var vendorSubGroup = VendorSubGroups.SingleOrDefault(x => x.Id == vendorSubGroupId.Trim());
        if (vendorSubGroup == null)
        {
            throw new DomainException($"{ExceptionConsts.EntitiyNotFound}");
        }

        return vendorSubGroup;
    }

}




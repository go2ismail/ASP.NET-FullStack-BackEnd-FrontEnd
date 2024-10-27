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

public class CustomerGroup : BaseEntityCommon, IAggregateRoot
{
    public ICollection<CustomerSubGroup> CustomerSubGroups { get; set; } = new Collection<CustomerSubGroup>();

    public CustomerGroup() : base() { } //for EF Core
    public CustomerGroup(
        string? userId,
        string name,
        string? description
        ) : base(userId, name, description)
    {
        CustomerSubGroups = new Collection<CustomerSubGroup>();
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
        foreach (CustomerSubGroup subGroup in CustomerSubGroups)
        {
            DeleteSubGroup(userId, subGroup.Id);
        }
        SetAsDeleted();
        SetAudit(userId);
    }


    public CustomerSubGroup CreateSubGroup(
        string? userId,
        string name,
        string? description
        )
    {
        if (CustomerSubGroups.Any(x => string.Equals(x.Name.Trim(), name.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new DomainException($"{ExceptionConsts.EntitiyAlreadyExists}: {name}");
        }

        var customerSubGroup = new CustomerSubGroup(userId, this.Id, name, description);
        CustomerSubGroups.Add(customerSubGroup);

        return customerSubGroup;
    }

    public CustomerSubGroup UpdateSubGroup(
        string? userId,
        string customerSubGroupId,
        string name,
        string? description
    )
    {
        var customerSubGroup = CustomerSubGroups.SingleOrDefault(x => x.Id == customerSubGroupId.Trim());
        if (customerSubGroup == null)
        {
            throw new DomainException($"{ExceptionConsts.EntitiyNotFound}");
        }

        if (CustomerSubGroups.Any(x => string.Equals(x.Name.Trim(), name.Trim(), StringComparison.OrdinalIgnoreCase)
       && x.Id != customerSubGroupId.Trim()))
        {
            throw new DomainException($"{ExceptionConsts.EntitiyAlreadyExists}: {name}");
        }


        customerSubGroup.Update(userId, name, description);

        return customerSubGroup;
    }


    public CustomerSubGroup DeleteSubGroup(string? userId, string customerSubGroupId)
    {
        var customerSubGroup = CustomerSubGroups.SingleOrDefault(x => x.Id == customerSubGroupId.Trim());
        if (customerSubGroup == null)
        {
            throw new DomainException($"{ExceptionConsts.EntitiyNotFound}");
        }

        customerSubGroup.Delete(userId);

        return customerSubGroup;
    }
    public CustomerSubGroup GetSubGroup(string customerSubGroupId)
    {
        var customerSubGroup = CustomerSubGroups.SingleOrDefault(x => x.Id == customerSubGroupId.Trim());
        if (customerSubGroup == null)
        {
            throw new DomainException($"{ExceptionConsts.EntitiyNotFound}");
        }

        return customerSubGroup;
    }

}




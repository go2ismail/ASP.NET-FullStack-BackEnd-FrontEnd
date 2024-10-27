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

public class Vendor : BaseEntityAdvance, IAggregateRoot
{
    public string VendorGroupId { get; set; } = null!;
    public string? VendorSubGroupId { get; set; }
    public string Street { get; set; } = null!;
    public string City { get; set; } = null!;
    public string StateOrProvince { get; set; } = null!;
    public string ZipCode { get; set; } = null!;
    public string? Country { get; set; }
    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Website { get; set; }
    public ICollection<VendorContact> VendorContacts { get; set; } = new Collection<VendorContact>();


    public Vendor() : base() { } //for EF Core
    public Vendor(
        string? userId,
        string code,
        string name,
        string? description,
        string vendorGroupId,
        string? vendorSubGroupId,
        string street,
        string city,
        string stateOrProvince,
        string zipCode,
        string? country,
        string phone,
        string email,
        string? website) : base(userId, code, name, description)
    {
        VendorGroupId = vendorGroupId.Trim();
        VendorSubGroupId = vendorSubGroupId?.Trim();
        Street = street.Trim();
        City = city.Trim();
        StateOrProvince = stateOrProvince.Trim();
        ZipCode = zipCode.Trim();
        Country = country?.Trim();
        Phone = phone.Trim();
        Email = email.Trim();
        Website = website?.Trim();
        VendorContacts = new Collection<VendorContact>();
    }




    public void Update(
        string? userId,
        string name,
        string? description,
        string vendorGroupId,
        string? vendorSubGroupId,
        string street,
        string city,
        string stateOrProvince,
        string zipCode,
        string? country,
        string phone,
        string email,
        string? website
        )
    {
        Name = name.Trim();
        Description = description?.Trim();
        VendorGroupId = vendorGroupId.Trim();
        VendorSubGroupId = vendorSubGroupId?.Trim();
        Street = street.Trim();
        City = city.Trim();
        StateOrProvince = stateOrProvince.Trim();
        ZipCode = zipCode.Trim();
        Country = country?.Trim();
        Phone = phone.Trim();
        Email = email.Trim();
        Website = website?.Trim();

        SetAudit(userId);
    }

    public void Delete(
        string? userId
        )
    {
        foreach (VendorContact contact in VendorContacts)
        {
            DeleteContact(userId, contact.Id);
        }
        SetAsDeleted();
        SetAudit(userId);
    }


    public VendorContact CreateContact(
        string? userId,
        string firstName,
        string lastName,
        string genderId,
        string? description,
        string jobTitle,
        string? mobilePhone,
        string? socialMedia,
        string? address,
        string? city,
        string? stateOrProvince,
        string? zipCode,
        string? country,
        string phone,
        string email,
        string? website
        )
    {
        if (VendorContacts.Any(x => string.Equals(x.FirstName.Trim(), firstName.Trim(), StringComparison.OrdinalIgnoreCase)
            && string.Equals(x.LastName.Trim(), lastName.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new DomainException($"{ExceptionConsts.EntitiyAlreadyExists}: {firstName} {lastName}");
        }

        if (VendorContacts.Any(x => string.Equals(x.Email.Trim(), email.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new DomainException($"{ExceptionConsts.EntitiyAlreadyExists}: {email}");
        }


        var vendorContact = new VendorContact(
            userId,
            this.Id,
            firstName,
            lastName,
            genderId,
            description,
            jobTitle,
            mobilePhone,
            socialMedia,
            address,
            city,
            stateOrProvince,
            zipCode,
            country,
            phone,
            email,
            website);

        VendorContacts.Add(vendorContact);

        return vendorContact;
    }

    public VendorContact UpdateContact(
            string? userId,
            string vendorContactId,
            string firstName,
            string lastName,
            string jobTitle,
            string genderId,
            string email,
            string? description,
            string? mobilePhone,
            string? socialMedia,
            string? address,
            string? city,
            string? stateOrProvince,
            string? zipCode,
            string? country,
            string phone,
            string? website
        )
    {

        var vendorContact = VendorContacts.SingleOrDefault(x => x.Id == vendorContactId.Trim());
        if (vendorContact == null)
        {
            throw new DomainException($"{ExceptionConsts.EntitiyNotFound}");
        }
        if (VendorContacts.Any(x => string.Equals(x.FirstName.Trim(), firstName.Trim(), StringComparison.OrdinalIgnoreCase)
            && string.Equals(x.LastName.Trim(), lastName.Trim(), StringComparison.OrdinalIgnoreCase)
            && x.Id != vendorContactId))
        {
            throw new DomainException($"{ExceptionConsts.EntitiyAlreadyExists}: {firstName} {lastName}");
        }

        if (VendorContacts.Any(x => string.Equals(x.Email.Trim(), email.Trim(), StringComparison.OrdinalIgnoreCase)
            && x.Id != vendorContactId))
        {
            throw new DomainException($"{ExceptionConsts.EntitiyAlreadyExists}: {email}");
        }


        vendorContact.Update(
            userId,
            this.Id,
            firstName,
            lastName,
            genderId,
            description,
            jobTitle,
            mobilePhone,
            socialMedia,
            address,
            city,
            stateOrProvince,
            zipCode,
            country,
            phone,
            email,
            website);

        return vendorContact;
    }

    public VendorContact DeleteContact(string? userId, string vendorContactId)
    {
        var vendorContact = VendorContacts.SingleOrDefault(x => x.Id == vendorContactId);
        if (vendorContact == null)
        {
            throw new DomainException($"{ExceptionConsts.EntitiyNotFound}");
        }

        vendorContact.Delete(userId);

        return vendorContact;
    }
    public VendorContact GetContact(string vendorContactId)
    {
        var vendorContact = VendorContacts.SingleOrDefault(x => x.Id == vendorContactId.Trim());
        if (vendorContact == null)
        {
            throw new DomainException($"{ExceptionConsts.EntitiyNotFound}");
        }

        return vendorContact;
    }
}



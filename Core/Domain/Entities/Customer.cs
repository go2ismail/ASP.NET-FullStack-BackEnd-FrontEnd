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

public class Customer : BaseEntityAdvance, IAggregateRoot
{
    public string CustomerGroupId { get; set; } = null!;
    public string? CustomerSubGroupId { get; set; }
    public string Street { get; set; } = null!;
    public string City { get; set; } = null!;
    public string StateOrProvince { get; set; } = null!;
    public string ZipCode { get; set; } = null!;
    public string? Country { get; set; }
    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Website { get; set; }
    public ICollection<CustomerContact> CustomerContacts { get; set; } = new Collection<CustomerContact>();


    public Customer() : base() { } //for EF Core
    public Customer(
        string? userId,
        string code,
        string name,
        string? description,
        string customerGroupId,
        string? customerSubGroupId,
        string street,
        string city,
        string stateOrProvince,
        string zipCode,
        string? country,
        string phone,
        string email,
        string? website) : base(userId, code, name, description)
    {
        CustomerGroupId = customerGroupId.Trim();
        CustomerSubGroupId = customerSubGroupId?.Trim();
        Street = street.Trim();
        City = city.Trim();
        StateOrProvince = stateOrProvince.Trim();
        ZipCode = zipCode.Trim();
        Country = country?.Trim();
        Phone = phone.Trim();
        Email = email.Trim();
        Website = website?.Trim();
        CustomerContacts = new Collection<CustomerContact>();
    }




    public void Update(
        string? userId,
        string name,
        string? description,
        string customerGroupId,
        string? customerSubGroupId,
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
        CustomerGroupId = customerGroupId.Trim();
        CustomerSubGroupId = customerSubGroupId?.Trim();
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
        foreach (CustomerContact contact in CustomerContacts)
        {
            DeleteContact(userId, contact.Id);
        }
        SetAsDeleted();
        SetAudit(userId);
    }


    public CustomerContact CreateContact(
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

        if (CustomerContacts.Any(x => string.Equals(x.FirstName.Trim(), firstName.Trim(), StringComparison.OrdinalIgnoreCase)
            && string.Equals(x.LastName.Trim(), lastName.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new DomainException($"{ExceptionConsts.EntitiyAlreadyExists}: {firstName} {lastName}");
        }

        if (CustomerContacts.Any(x => string.Equals(x.Email.Trim(), email.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new DomainException($"{ExceptionConsts.EntitiyAlreadyExists}: {email}");
        }


        var customerContact = new CustomerContact(
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

        CustomerContacts.Add(customerContact);

        return customerContact;
    }

    public CustomerContact UpdateContact(
            string? userId,
            string customerContactId,
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

        var customerContact = CustomerContacts.SingleOrDefault(x => x.Id == customerContactId.Trim());
        if (customerContact == null)
        {
            throw new DomainException($"{ExceptionConsts.EntitiyNotFound}");
        }
        if (CustomerContacts.Any(x => string.Equals(x.FirstName.Trim(), firstName.Trim(), StringComparison.OrdinalIgnoreCase)
            && string.Equals(x.LastName.Trim(), lastName.Trim(), StringComparison.OrdinalIgnoreCase)
            && x.Id != customerContactId))
        {
            throw new DomainException($"{ExceptionConsts.EntitiyAlreadyExists}: {firstName} {lastName}");
        }

        if (CustomerContacts.Any(x => string.Equals(x.Email.Trim(), email.Trim(), StringComparison.OrdinalIgnoreCase)
            && x.Id != customerContactId))
        {
            throw new DomainException($"{ExceptionConsts.EntitiyAlreadyExists}: {email}");
        }



        customerContact.Update(
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

        return customerContact;
    }

    public CustomerContact DeleteContact(string? userId, string customerContactId)
    {
        var customerContact = CustomerContacts.SingleOrDefault(x => x.Id == customerContactId);
        if (customerContact == null)
        {
            throw new DomainException($"{ExceptionConsts.EntitiyNotFound}");
        }

        customerContact.Delete(userId);

        return customerContact;
    }

    public CustomerContact GetContact(string customerContactId)
    {
        var customerContact = CustomerContacts.SingleOrDefault(x => x.Id == customerContactId.Trim());
        if (customerContact == null)
        {
            throw new DomainException($"{ExceptionConsts.EntitiyNotFound}");
        }

        return customerContact;
    }

}



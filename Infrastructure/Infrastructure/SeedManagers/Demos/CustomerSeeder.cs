// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.CQS.Queries;
using Application.Services.Externals;
using Application.Services.Repositories;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;

using System.Text.Json;

namespace Infrastructure.SeedManagers.Demos;

public class CustomerSeeder
{
    private readonly INumberSequenceService _numberSequenceService;
    private readonly IBaseCommandRepository<Customer> _customer;
    private readonly IBaseCommandRepository<CustomerGroup> _customerGroup;
    private readonly IBaseCommandRepository<Gender> _gender;
    private readonly IUnitOfWork _unitOfWork;

    public CustomerSeeder(
        INumberSequenceService numberSequenceService,
        IBaseCommandRepository<Customer> customer,
        IBaseCommandRepository<CustomerGroup> customerGroup,
        IBaseCommandRepository<Gender> gender,
        IUnitOfWork unitOfWork)
    {
        _numberSequenceService = numberSequenceService;
        _customer = customer;
        _customerGroup = customerGroup;
        _gender = gender;
        _unitOfWork = unitOfWork;
    }

    public async Task GenerateDataAsync()
    {
        string customerJson = """
        [
            {
                "Code": "CUST001", "Name": "MediCare Health", "Street": "123 Elm St", 
                "City": "New York", "StateOrProvince": "NY", "ZipCode": "10001", "Phone": "1234567890", 
                "Email": "info@medicare.com",
                "CustomerContacts": [
                    {"FirstName": "Alex", "LastName": "Smith", "JobTitle": "Director", "Phone": "1234567890", "Email": "alex.smith@medicare.com"},
                    {"FirstName": "Jamie", "LastName": "Taylor", "JobTitle": "Manager", "Phone": "0987654321", "Email": "jamie.taylor@medicare.com"}
                ]
            },
            {
                "Code": "CUST002", "Name": "Defense Systems", "Street": "456 Oak St", 
                "City": "Chicago", "StateOrProvince": "IL", "ZipCode": "60601", "Phone": "0987654321", 
                "Email": "contact@defensesystems.com",
                "CustomerContacts": [
                    {"FirstName": "Jordan", "LastName": "Reed", "JobTitle": "Commander", "Phone": "1122334455", "Email": "jordan.reed@defensesystems.com"}
                ]
            },
            {
                "Code": "CUST003", "Name": "EduWorld", "Street": "789 Pine St", 
                "City": "Los Angeles", "StateOrProvince": "CA", "ZipCode": "90001", "Phone": "1122334455", 
                "Email": "support@eduworld.com",
                "CustomerContacts": [
                    {"FirstName": "Taylor", "LastName": "Lee", "JobTitle": "Principal", "Phone": "2233445566", "Email": "taylor.lee@eduworld.com"}
                ]
            },
            {
                "Code": "CUST004", "Name": "EngiPro Engineering", "Street": "101 Maple St", 
                "City": "Houston", "StateOrProvince": "TX", "ZipCode": "77001", "Phone": "2233445566", 
                "Email": "services@engipro.com",
                "CustomerContacts": [
                    {"FirstName": "Morgan", "LastName": "Carter", "JobTitle": "Chief Engineer", "Phone": "3344556677", "Email": "morgan.carter@engipro.com"}
                ]
            },
            {
                "Code": "CUST005", "Name": "Health Solutions", "Street": "202 Cedar St", 
                "City": "Phoenix", "StateOrProvince": "AZ", "ZipCode": "85001", "Phone": "3344556677", 
                "Email": "info@healthsolutions.com",
                "CustomerContacts": [
                    {"FirstName": "Riley", "LastName": "Brown", "JobTitle": "VP of Operations", "Phone": "4455667788", "Email": "riley.brown@healthsolutions.com"}
                ]
            },
            {
                "Code": "CUST006", "Name": "Tactical Gear Co.", "Street": "303 Birch St", 
                "City": "Philadelphia", "StateOrProvince": "PA", "ZipCode": "19101", "Phone": "4455667788", 
                "Email": "sales@tacticalgear.com",
                "CustomerContacts": [
                    {"FirstName": "Avery", "LastName": "Clark", "JobTitle": "Sales Manager", "Phone": "5566778899", "Email": "avery.clark@tacticalgear.com"}
                ]
            },
            {
                "Code": "CUST007", "Name": "Bright Minds Academy", "Street": "404 Spruce St", 
                "City": "San Antonio", "StateOrProvince": "TX", "ZipCode": "78201", "Phone": "5566778899", 
                "Email": "contact@brightminds.edu",
                "CustomerContacts": [
                    {"FirstName": "Charlie", "LastName": "Davis", "JobTitle": "Head of Admissions", "Phone": "6677889900", "Email": "charlie.davis@brightminds.edu"}
                ]
            },
            {
                "Code": "CUST008", "Name": "BuildRight Contractors", "Street": "505 Walnut St", 
                "City": "San Diego", "StateOrProvince": "CA", "ZipCode": "92101", "Phone": "6677889900", 
                "Email": "info@buildright.com",
                "CustomerContacts": [
                    {"FirstName": "Cameron", "LastName": "Martinez", "JobTitle": "Project Manager", "Phone": "7788990011", "Email": "cameron.martinez@buildright.com"}
                ]
            },
            {
                "Code": "CUST009", "Name": "CarePlus", "Street": "606 Ash St", 
                "City": "Dallas", "StateOrProvince": "TX", "ZipCode": "75201", "Phone": "7788990011", 
                "Email": "info@careplus.com",
                "CustomerContacts": [
                    {"FirstName": "Skyler", "LastName": "Evans", "JobTitle": "Operations Manager", "Phone": "8899001122", "Email": "skyler.evans@careplus.com"}
                ]
            },
            {
                "Code": "CUST010", "Name": "DefenseTech", "Street": "707 Poplar St", 
                "City": "Austin", "StateOrProvince": "TX", "ZipCode": "73301", "Phone": "8899001122", 
                "Email": "contact@defensetech.com",
                "CustomerContacts": [
                    {"FirstName": "Casey", "LastName": "Jordan", "JobTitle": "Logistics Coordinator", "Phone": "9900112233", "Email": "casey.jordan@defensetech.com"}
                ]
            }
        ]
        """;

        var customers = JsonSerializer.Deserialize<List<Customer>>(customerJson);

        if (customers != null)
        {
            var random = new Random();

            var genders = await _gender.GetQuery()
                .ApplyIsDeletedFilter()
                .AsNoTracking()
                .ToListAsync();

            var customerGroup = await _customerGroup.GetQuery()
                .ApplyIsDeletedFilter()
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Name == "Default");

            foreach (var customer in customers)
            {
                var code = _numberSequenceService.GenerateNumberSequence(
                    null,
                    nameof(Customer),
                    null,
                    "CST");

                var entity = new Customer(
                    null, // userId
                    code,
                    customer.Name,
                    null, // description
                    customerGroup!.Id,
                    null, // customerSubGroupId
                    customer.Street,
                    customer.City,
                    customer.StateOrProvince,
                    customer.ZipCode,
                    null, // country
                    customer.Phone,
                    customer.Email,
                    null // website
                );


                foreach (var contact in customer.CustomerContacts)
                {
                    var childEntity = entity.CreateContact(
                        null, // userId
                        contact.FirstName,
                        contact.LastName,
                        genders[random.Next(0, genders.Count)].Id,
                        null, // description
                        contact.JobTitle,
                        null, // mobilePhone
                        null, // socialMedia
                        null, // address
                        null, // city
                        null, // stateOrProvince
                        null, // zipCode
                        null, // country
                        contact.Phone,
                        contact.Email,
                        null // website
                    );

                }

                await _customer.CreateAsync(entity);
                await _unitOfWork.SaveAsync();
            }


        }
    }

}

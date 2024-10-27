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

public class VendorSeeder(
    INumberSequenceService numberSequenceService,
    IBaseCommandRepository<Vendor> vendor,
    IBaseCommandRepository<VendorGroup> vendorGroup,
    IBaseCommandRepository<Gender> gender,
    IUnitOfWork unitOfWork)
{
    private readonly INumberSequenceService _numberSequenceService = numberSequenceService;
    private readonly IBaseCommandRepository<Vendor> _vendor = vendor;
    private readonly IBaseCommandRepository<VendorGroup> _vendorGroup = vendorGroup;
    private readonly IBaseCommandRepository<Gender> _gender = gender;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task GenerateDataAsync()
    {
        string vendorJson = """
        [
            {
                "Code": "VEND001", "Name": "Tech Supplies Inc.", "Street": "101 Tech Rd", 
                "City": "San Francisco", "StateOrProvince": "CA", "ZipCode": "94101", "Phone": "555-1234", 
                "Email": "info@techsupplies.com",
                "VendorContacts": [
                    {"FirstName": "Samuel", "LastName": "Adams", "JobTitle": "Sales Executive", "Phone": "555-5678", "Email": "samuel.adams@techsupplies.com"},
                    {"FirstName": "Sarah", "LastName": "Conner", "JobTitle": "Customer Support", "Phone": "555-8765", "Email": "sarah.conner@techsupplies.com"}
                ]
            },
            {
                "Code": "VEND002", "Name": "Office Essentials", "Street": "202 Office St", 
                "City": "Los Angeles", "StateOrProvince": "CA", "ZipCode": "90001", "Phone": "555-4321", 
                "Email": "contact@officeessentials.com",
                "VendorContacts": [
                    {"FirstName": "George", "LastName": "Miller", "JobTitle": "Account Manager", "Phone": "555-1111", "Email": "george.miller@officeessentials.com"}
                ]
            },
            {
                "Code": "VEND003", "Name": "Green Energy Co.", "Street": "303 Greenway Ave", 
                "City": "Austin", "StateOrProvince": "TX", "ZipCode": "73301", "Phone": "555-2222", 
                "Email": "info@greenenergy.com",
                "VendorContacts": [
                    {"FirstName": "Emily", "LastName": "Johnson", "JobTitle": "Project Manager", "Phone": "555-3333", "Email": "emily.johnson@greenenergy.com"}
                ]
            },
            {
                "Code": "VEND004", "Name": "Construction Supplies", "Street": "404 Builder St", 
                "City": "Chicago", "StateOrProvince": "IL", "ZipCode": "60601", "Phone": "555-4444", 
                "Email": "sales@constructionsupplies.com",
                "VendorContacts": [
                    {"FirstName": "Michael", "LastName": "Brown", "JobTitle": "Sales Manager", "Phone": "555-5555", "Email": "michael.brown@constructionsupplies.com"}
                ]
            },
            {
                "Code": "VEND005", "Name": "Cleaning Solutions", "Street": "505 Clean St", 
                "City": "Miami", "StateOrProvince": "FL", "ZipCode": "33101", "Phone": "555-6666", 
                "Email": "info@cleaningsolutions.com",
                "VendorContacts": [
                    {"FirstName": "Olivia", "LastName": "Davis", "JobTitle": "Operations Manager", "Phone": "555-7777", "Email": "olivia.davis@cleaningsolutions.com"}
                ]
            }
        ]
        """;

        var vendors = JsonSerializer.Deserialize<List<Vendor>>(vendorJson);

        if (vendors != null)
        {
            var random = new Random();

            var genders = await _gender.GetQuery()
                .ApplyIsDeletedFilter()
                .AsNoTracking()
                .ToListAsync();

            var vendorGroup = await _vendorGroup.GetQuery()
                .ApplyIsDeletedFilter()
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Name == "Default");

            foreach (var vendor in vendors)
            {
                var code = _numberSequenceService.GenerateNumberSequence(
                    null,
                    nameof(Vendor),
                    null,
                    "VND");

                var entity = new Vendor(
                    null, // userId
                    code,
                    vendor.Name,
                    null, // description
                    vendorGroup!.Id,
                    null, // vendorSubGroupId
                    vendor.Street,
                    vendor.City,
                    vendor.StateOrProvince,
                    vendor.ZipCode,
                    null, // country
                    vendor.Phone,
                    vendor.Email,
                    null // website
                );


                foreach (var contact in vendor.VendorContacts)
                {
                    entity.CreateContact(
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


                await _vendor.CreateAsync(entity);
                await _unitOfWork.SaveAsync();
            }

        }
    }
}

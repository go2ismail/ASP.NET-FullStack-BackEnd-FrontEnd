// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Common.Models;
using Application.Services.CQS.Queries;

using AutoMapper;

using Domain.Entities;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using System.Collections.ObjectModel;

namespace Application.Features.Customers.Queries;


public record GetPagedCustomerCustomerContactDto
{
    public string Id { get; init; } = null!;
    public string CustomerId { get; init; } = null!;
    public string? CustomerName { get; init; }
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string GenderId { get; init; } = null!;
    public string? GenderName { get; set; }
    public string? Description { get; init; }
    public string JobTitle { get; init; } = null!;
    public string? MobilePhone { get; init; }
    public string? SocialMedia { get; init; }
    public string? Address { get; init; }
    public string? City { get; init; }
    public string? StateOrProvince { get; init; }
    public string? ZipCode { get; init; }
    public string? Country { get; init; }
    public string Phone { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string? Website { get; init; }
}

public record GetPagedCustomerDto
{
    public string Id { get; init; } = null!;
    public string Code { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public string CustomerGroupId { get; init; } = null!;
    public string? CustomerGroupName { get; init; }
    public string? CustomerSubGroupId { get; init; }
    public string? CustomerSubGroupName { get; init; }
    public string Street { get; init; } = null!;
    public string City { get; init; } = null!;
    public string StateOrProvince { get; init; } = null!;
    public string ZipCode { get; init; } = null!;
    public string? Country { get; init; }
    public string Phone { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string? Website { get; init; }
    public ICollection<GetPagedCustomerCustomerContactDto> CustomerContacts { get; set; } = new Collection<GetPagedCustomerCustomerContactDto>();
}


public class GetPagedCustomerProfile : Profile
{
    public GetPagedCustomerProfile()
    {
        CreateMap<Customer, GetPagedCustomerDto>();
        CreateMap<CustomerContact, GetPagedCustomerCustomerContactDto>();
    }
}

public class GetPagedCustomerResult
{
    public PagedList<GetPagedCustomerDto>? Data { get; init; }
    public string Message { get; init; } = null!;
}

public class GetPagedCustomerRequest : IRequest<GetPagedCustomerResult>
{
    public IODataQueryOptions<Customer> QueryOptions { get; init; } = null!;
    public string SearchValue { get; init; } = null!;
    public bool IsDeleted { get; init; } = false;
}

public class GetPagedCustomerValidator : AbstractValidator<GetPagedCustomerRequest>
{
    public GetPagedCustomerValidator()
    {

        RuleFor(x => x.QueryOptions)
            .NotNull().WithMessage("Query options are required.");
    }
}


public class GetPagedCustomerHandler : IRequestHandler<GetPagedCustomerRequest, GetPagedCustomerResult>
{
    private readonly IMapper _mapper;
    private readonly IQueryContext _context;

    public GetPagedCustomerHandler(IMapper mapper, IQueryContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<GetPagedCustomerResult> Handle(GetPagedCustomerRequest request, CancellationToken cancellationToken)
    {
        // Initial Setup: Define common variables and base query
        var orderby = request.QueryOptions.GetOrderby();
        if (string.IsNullOrEmpty(orderby))
        {
            throw new ApplicationException("orderby must not empty");
        }
        int top = request.QueryOptions.GetTop();
        if (top <= 0)
        {
            throw new ApplicationException("top must not zero or less");
        }
        int skip = request.QueryOptions.GetSkip();
        if (skip < 0)
        {
            throw new ApplicationException("skip not negative");
        }

        int totalRecords = 0;
        string keywords = request.SearchValue;
        bool useDBSortingPaging = !string.IsNullOrEmpty(orderby) && orderby.Contains(".") ? false : true; //dot meaning: relation property: entityName.propertyName

        var baseQuery = _context.Customer
            .AsNoTracking()
            .ApplyIsDeletedFilter(request.IsDeleted)
            .AsQueryable();

        var customerQuery = baseQuery;

        // Database-Side Filtering (Attributes)
        var odataQuery = baseQuery.ApplyODataFilter(request.QueryOptions);
        customerQuery = odataQuery;

        // Combining Queries for Relationships
        if (!string.IsNullOrEmpty(keywords))
        {
            var customerGroupQuery = baseQuery.ApplyRelationFilter<Customer, CustomerGroup>(keywords, "CustomerGroupId", _context);
            var customerSubGroupQuery = baseQuery.ApplyRelationFilter<Customer, CustomerSubGroup>(keywords, "CustomerSubGroupId", _context);
            customerQuery = odataQuery.Union(customerGroupQuery).Union(customerSubGroupQuery);
        }

        // Sorting & Paging Logic:
        if (useDBSortingPaging)
        {
            // Apply sorting and paging in the database if sorting is based on field properties
            customerQuery = customerQuery.ApplyODataSorting(request.QueryOptions);
            totalRecords = await customerQuery.CountAsync(cancellationToken);
            customerQuery = customerQuery.ApplyODataPaging(request.QueryOptions, out skip, out top);
        }

        // Fetch Data & Map to DTOs
        var entities = await (
            from customer in customerQuery
            join customerGroup in _context.CustomerGroup.AsNoTracking()
                on customer.CustomerGroupId equals customerGroup.Id into customerGroupLookup
            from customerGroup in customerGroupLookup.DefaultIfEmpty()
            join customerSubGroup in _context.CustomerSubGroup.AsNoTracking()
                on customer.CustomerSubGroupId equals customerSubGroup.Id into customerSubGroupLookup
            from customerSubGroup in customerSubGroupLookup.DefaultIfEmpty()
            select new
            {
                customer,
                CustomerGroupName = customerGroup != null ? customerGroup.Name : null,
                CustomerSubGroupName = customerSubGroup != null ? customerSubGroup.Name : null
            }
        ).ToListAsync(cancellationToken);


        var dtos = entities.Select(entity =>
        {
            var dto = _mapper.Map<GetPagedCustomerDto>(entity.customer);
            return dto with
            {
                CustomerGroupName = entity.CustomerGroupName,
                CustomerSubGroupName = entity.CustomerSubGroupName,
                CustomerContacts = new List<GetPagedCustomerCustomerContactDto>()
            };
        }).ToList();

        // In-memory Sorting and Paging:
        if (!useDBSortingPaging)
        {
            // Apply sorting and paging in memory if sorting is based on relation properties
            dtos = dtos.ApplySorting(orderby);
            totalRecords = dtos.Count;
            dtos = dtos.ApplyPaging(skip, top);
        }

        foreach (var dto in dtos)
        {
            dto.CustomerContacts = GetCustomerContactsByCustomerId(dto.Id);
        }

        // Result Construction
        return new GetPagedCustomerResult
        {
            Data = new PagedList<GetPagedCustomerDto>(dtos, totalRecords, (top > 0) ? (skip / top) + 1 : 1, top),
            Message = "Success"
        };
    }

    private List<GetPagedCustomerCustomerContactDto> GetCustomerContactsByCustomerId(string customerId)
    {
        var customerContacts = (
            from contact in _context.CustomerContact.ApplyIsDeletedFilter().AsNoTracking()
            join gender in _context.Gender.AsNoTracking()
                on contact.GenderId equals gender.Id into genderLookup
            from gender in genderLookup.DefaultIfEmpty()
            where contact.CustomerId == customerId
            select new
            {
                contact,
                GenderName = gender != null ? gender.Name : null
            }
        ).ToList();


        var customerContactDtos = customerContacts.Select(c =>
        {
            var contactDto = _mapper.Map<GetPagedCustomerCustomerContactDto>(c.contact);
            contactDto.GenderName = c.GenderName;
            return contactDto;
        }).ToList();

        return customerContactDtos;
    }


}





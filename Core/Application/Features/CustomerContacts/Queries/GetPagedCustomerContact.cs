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

namespace Application.Features.CustomerContacts.Queries;



public record GetPagedCustomerContactDto
{
    public string Id { get; init; } = null!;
    public string CustomerId { get; init; } = null!;
    public string? CustomerName { get; init; }
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string GenderId { get; init; } = null!;
    public string? GenderName { get; init; }
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


public class GetPagedCustomerContactProfile : Profile
{
    public GetPagedCustomerContactProfile()
    {
        CreateMap<CustomerContact, GetPagedCustomerContactDto>();
    }
}

public class GetPagedCustomerContactResult
{
    public PagedList<GetPagedCustomerContactDto>? Data { get; init; }
    public string Message { get; init; } = null!;
}

public class GetPagedCustomerContactRequest : IRequest<GetPagedCustomerContactResult>
{
    public IODataQueryOptions<CustomerContact> QueryOptions { get; init; } = null!;
    public string SearchValue { get; init; } = null!;
    public bool IsDeleted { get; init; } = false;
}

public class GetPagedCustomerContactValidator : AbstractValidator<GetPagedCustomerContactRequest>
{
    public GetPagedCustomerContactValidator()
    {

        RuleFor(x => x.QueryOptions)
            .NotNull().WithMessage("Query options are required.");
    }
}


public class GetPagedCustomerContactHandler : IRequestHandler<GetPagedCustomerContactRequest, GetPagedCustomerContactResult>
{
    private readonly IMapper _mapper;
    private readonly IQueryContext _context;

    public GetPagedCustomerContactHandler(IMapper mapper, IQueryContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<GetPagedCustomerContactResult> Handle(GetPagedCustomerContactRequest request, CancellationToken cancellationToken)
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

        var baseQuery = _context.CustomerContact
            .AsNoTracking()
            .ApplyIsDeletedFilter(request.IsDeleted)
            .AsQueryable();

        var customerContactQuery = baseQuery;

        // Database-Side Filtering (Attributes)
        var odataQuery = baseQuery.ApplyODataFilter(request.QueryOptions);
        customerContactQuery = odataQuery;

        // Combining Queries for Relationships
        if (!string.IsNullOrEmpty(keywords))
        {
            var customerQuery = baseQuery.ApplyRelationFilter<CustomerContact, Customer>(keywords, "CustomerId", _context);
            var genderQuery = baseQuery.ApplyRelationFilter<CustomerContact, Gender>(keywords, "GenderId", _context);
            customerContactQuery = odataQuery.Union(customerQuery).Union(genderQuery);
        }

        // Sorting & Paging Logic:
        if (useDBSortingPaging)
        {
            // Apply sorting and paging in the database if sorting is based on field properties
            customerContactQuery = customerContactQuery.ApplyODataSorting(request.QueryOptions);
            totalRecords = await customerContactQuery.CountAsync(cancellationToken);
            customerContactQuery = customerContactQuery.ApplyODataPaging(request.QueryOptions, out skip, out top);
        }

        // Fetch Data & Map to DTOs
        var entities = await (
            from customerContact in customerContactQuery
            join customer in _context.Customer.AsNoTracking()
                on customerContact.CustomerId equals customer.Id into customerLookup
            from customer in customerLookup.DefaultIfEmpty()
            join gender in _context.Gender.AsNoTracking()
                on customerContact.GenderId equals gender.Id into genderLookup
            from gender in genderLookup.DefaultIfEmpty()
            select new
            {
                customerContact,
                CustomerName = customer != null ? customer.Name : null,
                GenderName = gender != null ? gender.Name : null
            }
        ).ToListAsync(cancellationToken);

        var dtos = entities.Select(entity =>
        {
            var dto = _mapper.Map<GetPagedCustomerContactDto>(entity.customerContact);
            return dto with
            {
                CustomerName = entity.CustomerName,
                GenderName = entity.GenderName
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

        // Result Construction
        return new GetPagedCustomerContactResult
        {
            Data = new PagedList<GetPagedCustomerContactDto>(dtos, totalRecords, (top > 0) ? (skip / top) + 1 : 1, top),
            Message = "Success"
        };
    }

}





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

namespace Application.Features.VendorContacts.Queries;



public record GetPagedVendorContactDto
{
    public string Id { get; init; } = null!;
    public string VendorId { get; init; } = null!;
    public string? VendorName { get; init; }
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


public class GetPagedVendorContactProfile : Profile
{
    public GetPagedVendorContactProfile()
    {
        CreateMap<VendorContact, GetPagedVendorContactDto>();
    }
}

public class GetPagedVendorContactResult
{
    public PagedList<GetPagedVendorContactDto>? Data { get; init; }
    public string Message { get; init; } = null!;
}

public class GetPagedVendorContactRequest : IRequest<GetPagedVendorContactResult>
{
    public IODataQueryOptions<VendorContact> QueryOptions { get; init; } = null!;
    public string SearchValue { get; init; } = null!;
    public bool IsDeleted { get; init; } = false;
}

public class GetPagedVendorContactValidator : AbstractValidator<GetPagedVendorContactRequest>
{
    public GetPagedVendorContactValidator()
    {

        RuleFor(x => x.QueryOptions)
            .NotNull().WithMessage("Query options are required.");
    }
}


public class GetPagedVendorContactHandler : IRequestHandler<GetPagedVendorContactRequest, GetPagedVendorContactResult>
{
    private readonly IMapper _mapper;
    private readonly IQueryContext _context;

    public GetPagedVendorContactHandler(IMapper mapper, IQueryContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<GetPagedVendorContactResult> Handle(GetPagedVendorContactRequest request, CancellationToken cancellationToken)
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

        var baseQuery = _context.VendorContact
            .AsNoTracking()
            .ApplyIsDeletedFilter(request.IsDeleted)
            .AsQueryable();

        var vendorContactQuery = baseQuery;

        // Database-Side Filtering (Attributes)
        var odataQuery = baseQuery.ApplyODataFilter(request.QueryOptions);
        vendorContactQuery = odataQuery;

        // Combining Queries for Relationships
        if (!string.IsNullOrEmpty(keywords))
        {
            var vendorQuery = baseQuery.ApplyRelationFilter<VendorContact, Vendor>(keywords, "VendorId", _context);
            var genderQuery = baseQuery.ApplyRelationFilter<VendorContact, Gender>(keywords, "GenderId", _context);
            vendorContactQuery = odataQuery.Union(vendorQuery).Union(genderQuery);
        }

        // Sorting & Paging Logic:
        if (useDBSortingPaging)
        {
            // Apply sorting and paging in the database if sorting is based on field properties
            vendorContactQuery = vendorContactQuery.ApplyODataSorting(request.QueryOptions);
            totalRecords = await vendorContactQuery.CountAsync(cancellationToken);
            vendorContactQuery = vendorContactQuery.ApplyODataPaging(request.QueryOptions, out skip, out top);
        }

        // Fetch Data & Map to DTOs
        var entities = await (
            from vendorContact in vendorContactQuery
            join vendor in _context.Vendor.AsNoTracking()
                on vendorContact.VendorId equals vendor.Id into vendorLookup
            from vendor in vendorLookup.DefaultIfEmpty()
            join gender in _context.Gender.AsNoTracking()
                on vendorContact.GenderId equals gender.Id into genderLookup
            from gender in genderLookup.DefaultIfEmpty()
            select new
            {
                vendorContact,
                VendorName = vendor != null ? vendor.Name : null,
                GenderName = gender != null ? gender.Name : null
            }
        ).ToListAsync(cancellationToken);

        var dtos = entities.Select(entity =>
        {
            var dto = _mapper.Map<GetPagedVendorContactDto>(entity.vendorContact);
            return dto with
            {
                VendorName = entity.VendorName,
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
        return new GetPagedVendorContactResult
        {
            Data = new PagedList<GetPagedVendorContactDto>(dtos, totalRecords, (top > 0) ? (skip / top) + 1 : 1, top),
            Message = "Success"
        };
    }

}





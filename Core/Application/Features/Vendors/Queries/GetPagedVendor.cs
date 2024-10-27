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

namespace Application.Features.Vendors.Queries;


public record GetPagedVendorVendorContactDto
{
    public string Id { get; init; } = null!;
    public string VendorId { get; init; } = null!;
    public string? VendorName { get; init; }
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

public record GetPagedVendorDto
{
    public string Id { get; init; } = null!;
    public string Code { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public string VendorGroupId { get; init; } = null!;
    public string? VendorGroupName { get; init; }
    public string? VendorSubGroupId { get; init; }
    public string? VendorSubGroupName { get; init; }
    public string Street { get; init; } = null!;
    public string City { get; init; } = null!;
    public string StateOrProvince { get; init; } = null!;
    public string ZipCode { get; init; } = null!;
    public string? Country { get; init; }
    public string Phone { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string? Website { get; init; }
    public ICollection<GetPagedVendorVendorContactDto> VendorContacts { get; set; } = new Collection<GetPagedVendorVendorContactDto>();
}


public class GetPagedVendorProfile : Profile
{
    public GetPagedVendorProfile()
    {
        CreateMap<Vendor, GetPagedVendorDto>();
        CreateMap<VendorContact, GetPagedVendorVendorContactDto>();
    }
}

public class GetPagedVendorResult
{
    public PagedList<GetPagedVendorDto>? Data { get; init; }
    public string Message { get; init; } = null!;
}

public class GetPagedVendorRequest : IRequest<GetPagedVendorResult>
{
    public IODataQueryOptions<Vendor> QueryOptions { get; init; } = null!;
    public string SearchValue { get; init; } = null!;
    public bool IsDeleted { get; init; } = false;
}

public class GetPagedVendorValidator : AbstractValidator<GetPagedVendorRequest>
{
    public GetPagedVendorValidator()
    {

        RuleFor(x => x.QueryOptions)
            .NotNull().WithMessage("Query options are required.");
    }
}


public class GetPagedVendorHandler : IRequestHandler<GetPagedVendorRequest, GetPagedVendorResult>
{
    private readonly IMapper _mapper;
    private readonly IQueryContext _context;

    public GetPagedVendorHandler(IMapper mapper, IQueryContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<GetPagedVendorResult> Handle(GetPagedVendorRequest request, CancellationToken cancellationToken)
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

        var baseQuery = _context.Vendor
            .AsNoTracking()
            .ApplyIsDeletedFilter(request.IsDeleted)
            .AsQueryable();

        var vendorQuery = baseQuery;

        // Database-Side Filtering (Attributes)
        var odataQuery = baseQuery.ApplyODataFilter(request.QueryOptions);
        vendorQuery = odataQuery;

        // Combining Queries for Relationships
        if (!string.IsNullOrEmpty(keywords))
        {
            var vendorGroupQuery = baseQuery.ApplyRelationFilter<Vendor, VendorGroup>(keywords, "VendorGroupId", _context);
            var vendorSubGroupQuery = baseQuery.ApplyRelationFilter<Vendor, VendorSubGroup>(keywords, "VendorSubGroupId", _context);
            vendorQuery = odataQuery.Union(vendorGroupQuery).Union(vendorSubGroupQuery);
        }

        // Sorting & Paging Logic:
        if (useDBSortingPaging)
        {
            // Apply sorting and paging in the database if sorting is based on field properties
            vendorQuery = vendorQuery.ApplyODataSorting(request.QueryOptions);
            totalRecords = await vendorQuery.CountAsync(cancellationToken);
            vendorQuery = vendorQuery.ApplyODataPaging(request.QueryOptions, out skip, out top);
        }

        // Fetch Data & Map to DTOs
        var entities = await (
            from vendor in vendorQuery
            join vendorGroup in _context.VendorGroup.AsNoTracking()
                on vendor.VendorGroupId equals vendorGroup.Id into vendorGroupLookup
            from vendorGroup in vendorGroupLookup.DefaultIfEmpty()
            join vendorSubGroup in _context.VendorSubGroup.AsNoTracking()
                on vendor.VendorSubGroupId equals vendorSubGroup.Id into vendorSubGroupLookup
            from vendorSubGroup in vendorSubGroupLookup.DefaultIfEmpty()
            select new
            {
                vendor,
                VendorGroupName = vendorGroup != null ? vendorGroup.Name : null,
                VendorSubGroupName = vendorSubGroup != null ? vendorSubGroup.Name : null
            }
        ).ToListAsync(cancellationToken);


        var dtos = entities.Select(entity =>
        {
            var dto = _mapper.Map<GetPagedVendorDto>(entity.vendor);
            return dto with
            {
                VendorGroupName = entity.VendorGroupName,
                VendorSubGroupName = entity.VendorSubGroupName,
                VendorContacts = new List<GetPagedVendorVendorContactDto>()
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
            dto.VendorContacts = GetVendorContactsByVendorId(dto.Id);
        }

        // Result Construction
        return new GetPagedVendorResult
        {
            Data = new PagedList<GetPagedVendorDto>(dtos, totalRecords, (top > 0) ? (skip / top) + 1 : 1, top),
            Message = "Success"
        };
    }

    private List<GetPagedVendorVendorContactDto> GetVendorContactsByVendorId(string vendorId)
    {
        var vendorContacts = (
            from contact in _context.VendorContact.ApplyIsDeletedFilter().AsNoTracking()
            join gender in _context.Gender.AsNoTracking()
                on contact.GenderId equals gender.Id into genderLookup
            from gender in genderLookup.DefaultIfEmpty()
            where contact.VendorId == vendorId
            select new
            {
                contact,
                GenderName = gender != null ? gender.Name : null
            }
        ).ToList();


        var vendorContactDtos = vendorContacts.Select(c =>
        {
            var contactDto = _mapper.Map<GetPagedVendorVendorContactDto>(c.contact);
            contactDto.GenderName = c.GenderName;
            return contactDto;
        }).ToList();

        return vendorContactDtos;
    }


}





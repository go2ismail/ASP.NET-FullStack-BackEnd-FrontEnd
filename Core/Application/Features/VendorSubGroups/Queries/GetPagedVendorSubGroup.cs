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

namespace Application.Features.VendorSubGroups.Queries;



public record GetPagedVendorSubGroupDto
{
    public string Id { get; init; } = null!;
    public string VendorGroupId { get; init; } = null!;
    public string? VendorGroupName { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
}


public class GetPagedVendorSubGroupProfile : Profile
{
    public GetPagedVendorSubGroupProfile()
    {
        CreateMap<VendorSubGroup, GetPagedVendorSubGroupDto>();
    }
}

public class GetPagedVendorSubGroupResult
{
    public PagedList<GetPagedVendorSubGroupDto>? Data { get; init; }
    public string Message { get; init; } = null!;
}

public class GetPagedVendorSubGroupRequest : IRequest<GetPagedVendorSubGroupResult>
{
    public IODataQueryOptions<VendorSubGroup> QueryOptions { get; init; } = null!;
    public string SearchValue { get; init; } = null!;
    public bool IsDeleted { get; init; } = false;
}

public class GetPagedVendorSubGroupValidator : AbstractValidator<GetPagedVendorSubGroupRequest>
{
    public GetPagedVendorSubGroupValidator()
    {

        RuleFor(x => x.QueryOptions)
            .NotNull().WithMessage("Query options are required.");
    }
}


public class GetPagedVendorSubGroupHandler : IRequestHandler<GetPagedVendorSubGroupRequest, GetPagedVendorSubGroupResult>
{
    private readonly IMapper _mapper;
    private readonly IQueryContext _context;

    public GetPagedVendorSubGroupHandler(IMapper mapper, IQueryContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<GetPagedVendorSubGroupResult> Handle(GetPagedVendorSubGroupRequest request, CancellationToken cancellationToken)
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

        var baseQuery = _context.VendorSubGroup
            .AsNoTracking()
            .ApplyIsDeletedFilter(request.IsDeleted)
            .AsQueryable();

        var vendorSubGroupQuery = baseQuery;

        // Database-Side Filtering (Attributes)
        var odataQuery = baseQuery.ApplyODataFilter(request.QueryOptions);
        vendorSubGroupQuery = odataQuery;

        // Combining Queries for Relationships
        if (!string.IsNullOrEmpty(keywords))
        {
            var vendorGroupQuery = baseQuery.ApplyRelationFilter<VendorSubGroup, VendorGroup>(keywords, "VendorGroupId", _context);
            vendorSubGroupQuery = odataQuery.Union(vendorGroupQuery);
        }

        // Sorting & Paging Logic:
        if (useDBSortingPaging)
        {
            // Apply sorting and paging in the database if sorting is based on field properties
            vendorSubGroupQuery = vendorSubGroupQuery.ApplyODataSorting(request.QueryOptions);
            totalRecords = await vendorSubGroupQuery.CountAsync(cancellationToken);
            vendorSubGroupQuery = vendorSubGroupQuery.ApplyODataPaging(request.QueryOptions, out skip, out top);
        }

        // Fetch Data & Map to DTOs
        var entities = await (
            from vendorSubGroup in vendorSubGroupQuery
            join vendorGroup in _context.VendorGroup.AsNoTracking()
                on vendorSubGroup.VendorGroupId equals vendorGroup.Id into vendorGroupLookup
            from vendorGroup in vendorGroupLookup.DefaultIfEmpty()
            select new
            {
                vendorSubGroup,
                VendorGroupName = vendorGroup != null ? vendorGroup.Name : null
            }
        ).ToListAsync(cancellationToken);

        var dtos = entities.Select(entity =>
        {
            var dto = _mapper.Map<GetPagedVendorSubGroupDto>(entity.vendorSubGroup);
            return dto with
            {
                VendorGroupName = entity.VendorGroupName
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
        return new GetPagedVendorSubGroupResult
        {
            Data = new PagedList<GetPagedVendorSubGroupDto>(dtos, totalRecords, (top > 0) ? (skip / top) + 1 : 1, top),
            Message = "Success"
        };
    }


}





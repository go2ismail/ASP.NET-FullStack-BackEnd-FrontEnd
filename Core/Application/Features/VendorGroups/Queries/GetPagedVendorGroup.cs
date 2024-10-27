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

namespace Application.Features.VendorGroups.Queries;



public record GetPagedVendorGroupDto
{
    public string Id { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public ICollection<VendorSubGroup> VendorSubGroups { get; set; } = new Collection<VendorSubGroup>();
}


public class GetPagedVendorGroupProfile : Profile
{
    public GetPagedVendorGroupProfile()
    {
        CreateMap<VendorGroup, GetPagedVendorGroupDto>();
    }
}

public class GetPagedVendorGroupResult
{
    public PagedList<GetPagedVendorGroupDto>? Data { get; init; }
    public string Message { get; init; } = null!;
}

public class GetPagedVendorGroupRequest : IRequest<GetPagedVendorGroupResult>
{
    public IODataQueryOptions<VendorGroup> QueryOptions { get; init; } = null!;
    public string SearchValue { get; init; } = null!;
    public bool IsDeleted { get; init; } = false;
}

public class GetPagedVendorGroupValidator : AbstractValidator<GetPagedVendorGroupRequest>
{
    public GetPagedVendorGroupValidator()
    {

        RuleFor(x => x.QueryOptions)
            .NotNull().WithMessage("Query options are required.");
    }
}


public class GetPagedVendorGroupHandler : IRequestHandler<GetPagedVendorGroupRequest, GetPagedVendorGroupResult>
{
    private readonly IMapper _mapper;
    private readonly IQueryContext _context;

    public GetPagedVendorGroupHandler(IMapper mapper, IQueryContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<GetPagedVendorGroupResult> Handle(GetPagedVendorGroupRequest request, CancellationToken cancellationToken)
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

        var baseQuery = _context.VendorGroup
            .AsNoTracking()
            .ApplyIsDeletedFilter(request.IsDeleted)
            .AsQueryable();

        var vendorGroupQuery = baseQuery;

        // Database-Side Filtering (Attributes)
        var odataQuery = baseQuery.ApplyODataFilter(request.QueryOptions);
        vendorGroupQuery = odataQuery;

        // Combining Queries for Relationships
        if (!string.IsNullOrEmpty(keywords))
        {
        }

        // Sorting & Paging Logic:
        if (useDBSortingPaging)
        {
            // Apply sorting and paging in the database if sorting is based on field properties
            vendorGroupQuery = vendorGroupQuery.ApplyODataSorting(request.QueryOptions);
            totalRecords = await vendorGroupQuery.CountAsync(cancellationToken);
            vendorGroupQuery = vendorGroupQuery.ApplyODataPaging(request.QueryOptions, out skip, out top);
        }

        // Fetch Data & Map to DTOs
        var entities = await (
            from vendorGroup in vendorGroupQuery
            select new
            {
                vendorGroup
            }
        ).ToListAsync(cancellationToken);

        var dtos = entities.Select(entity =>
        {
            var dto = _mapper.Map<GetPagedVendorGroupDto>(entity.vendorGroup);
            return dto with
            {
                VendorSubGroups = _context.VendorSubGroup
                    .AsQueryable()
                    .ApplyIsDeletedFilter()
                    .Where(x => x.VendorGroupId == entity.vendorGroup.Id)
                    .ToList()
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
        return new GetPagedVendorGroupResult
        {
            Data = new PagedList<GetPagedVendorGroupDto>(dtos, totalRecords, (top > 0) ? (skip / top) + 1 : 1, top),
            Message = "Success"
        };
    }

}





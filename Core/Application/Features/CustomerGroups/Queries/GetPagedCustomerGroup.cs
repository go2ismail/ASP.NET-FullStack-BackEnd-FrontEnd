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

namespace Application.Features.CustomerGroups.Queries;



public record GetPagedCustomerGroupDto
{
    public string Id { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public ICollection<CustomerSubGroup> CustomerSubGroups { get; set; } = new Collection<CustomerSubGroup>();
}


public class GetPagedCustomerGroupProfile : Profile
{
    public GetPagedCustomerGroupProfile()
    {
        CreateMap<CustomerGroup, GetPagedCustomerGroupDto>();
    }
}

public class GetPagedCustomerGroupResult
{
    public PagedList<GetPagedCustomerGroupDto>? Data { get; init; }
    public string Message { get; init; } = null!;
}

public class GetPagedCustomerGroupRequest : IRequest<GetPagedCustomerGroupResult>
{
    public IODataQueryOptions<CustomerGroup> QueryOptions { get; init; } = null!;
    public string SearchValue { get; init; } = null!;
    public bool IsDeleted { get; init; } = false;
}

public class GetPagedCustomerGroupValidator : AbstractValidator<GetPagedCustomerGroupRequest>
{
    public GetPagedCustomerGroupValidator()
    {

        RuleFor(x => x.QueryOptions)
            .NotNull().WithMessage("Query options are required.");
    }
}


public class GetPagedCustomerGroupHandler : IRequestHandler<GetPagedCustomerGroupRequest, GetPagedCustomerGroupResult>
{
    private readonly IMapper _mapper;
    private readonly IQueryContext _context;

    public GetPagedCustomerGroupHandler(IMapper mapper, IQueryContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<GetPagedCustomerGroupResult> Handle(GetPagedCustomerGroupRequest request, CancellationToken cancellationToken)
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

        var baseQuery = _context.CustomerGroup
            .AsNoTracking()
            .ApplyIsDeletedFilter(request.IsDeleted)
            .AsQueryable();

        var customerGroupQuery = baseQuery;

        // Database-Side Filtering (Attributes)
        var odataQuery = baseQuery.ApplyODataFilter(request.QueryOptions);
        customerGroupQuery = odataQuery;

        // Combining Queries for Relationships
        if (!string.IsNullOrEmpty(keywords))
        {
        }

        // Sorting & Paging Logic:
        if (useDBSortingPaging)
        {
            // Apply sorting and paging in the database if sorting is based on field properties
            customerGroupQuery = customerGroupQuery.ApplyODataSorting(request.QueryOptions);
            totalRecords = await customerGroupQuery.CountAsync(cancellationToken);
            customerGroupQuery = customerGroupQuery.ApplyODataPaging(request.QueryOptions, out skip, out top);
        }

        // Fetch Data & Map to DTOs
        var entities = await (
            from customerGroup in customerGroupQuery
            select new
            {
                customerGroup
            }
        ).ToListAsync(cancellationToken);

        var dtos = entities.Select(entity =>
        {
            var dto = _mapper.Map<GetPagedCustomerGroupDto>(entity.customerGroup);
            return dto with
            {
                CustomerSubGroups = _context.CustomerSubGroup
                    .AsQueryable()
                    .ApplyIsDeletedFilter()
                    .Where(x => x.CustomerGroupId == entity.customerGroup.Id)
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
        return new GetPagedCustomerGroupResult
        {
            Data = new PagedList<GetPagedCustomerGroupDto>(dtos, totalRecords, (top > 0) ? (skip / top) + 1 : 1, top),
            Message = "Success"
        };
    }

}





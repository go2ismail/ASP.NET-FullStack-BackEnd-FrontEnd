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

namespace Application.Features.CustomerSubGroups.Queries;



public record GetPagedCustomerSubGroupDto
{
    public string Id { get; init; } = null!;
    public string CustomerGroupId { get; init; } = null!;
    public string? CustomerGroupName { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
}


public class GetPagedCustomerSubGroupProfile : Profile
{
    public GetPagedCustomerSubGroupProfile()
    {
        CreateMap<CustomerSubGroup, GetPagedCustomerSubGroupDto>();
    }
}

public class GetPagedCustomerSubGroupResult
{
    public PagedList<GetPagedCustomerSubGroupDto>? Data { get; init; }
    public string Message { get; init; } = null!;
}

public class GetPagedCustomerSubGroupRequest : IRequest<GetPagedCustomerSubGroupResult>
{
    public IODataQueryOptions<CustomerSubGroup> QueryOptions { get; init; } = null!;
    public string SearchValue { get; init; } = null!;
    public bool IsDeleted { get; init; } = false;
}

public class GetPagedCustomerSubGroupValidator : AbstractValidator<GetPagedCustomerSubGroupRequest>
{
    public GetPagedCustomerSubGroupValidator()
    {

        RuleFor(x => x.QueryOptions)
            .NotNull().WithMessage("Query options are required.");
    }
}


public class GetPagedCustomerSubGroupHandler : IRequestHandler<GetPagedCustomerSubGroupRequest, GetPagedCustomerSubGroupResult>
{
    private readonly IMapper _mapper;
    private readonly IQueryContext _context;

    public GetPagedCustomerSubGroupHandler(IMapper mapper, IQueryContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<GetPagedCustomerSubGroupResult> Handle(GetPagedCustomerSubGroupRequest request, CancellationToken cancellationToken)
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

        var baseQuery = _context.CustomerSubGroup
            .AsNoTracking()
            .ApplyIsDeletedFilter(request.IsDeleted)
            .AsQueryable();

        var customerSubGroupQuery = baseQuery;

        // Database-Side Filtering (Attributes)
        var odataQuery = baseQuery.ApplyODataFilter(request.QueryOptions);
        customerSubGroupQuery = odataQuery;

        // Combining Queries for Relationships
        if (!string.IsNullOrEmpty(keywords))
        {
            var customerGroupQuery = baseQuery.ApplyRelationFilter<CustomerSubGroup, CustomerGroup>(keywords, "CustomerGroupId", _context);
            customerSubGroupQuery = odataQuery.Union(customerGroupQuery);
        }

        // Sorting & Paging Logic:
        if (useDBSortingPaging)
        {
            // Apply sorting and paging in the database if sorting is based on field properties
            customerSubGroupQuery = customerSubGroupQuery.ApplyODataSorting(request.QueryOptions);
            totalRecords = await customerSubGroupQuery.CountAsync(cancellationToken);
            customerSubGroupQuery = customerSubGroupQuery.ApplyODataPaging(request.QueryOptions, out skip, out top);
        }

        // Fetch Data & Map to DTOs
        var entities = await (
            from customerSubGroup in customerSubGroupQuery
            join customerGroup in _context.CustomerGroup.AsNoTracking()
                on customerSubGroup.CustomerGroupId equals customerGroup.Id into customerGroupLookup
            from customerGroup in customerGroupLookup.DefaultIfEmpty()
            select new
            {
                customerSubGroup,
                CustomerGroupName = customerGroup != null ? customerGroup.Name : null
            }
        ).ToListAsync(cancellationToken);

        var dtos = entities.Select(entity =>
        {
            var dto = _mapper.Map<GetPagedCustomerSubGroupDto>(entity.customerSubGroup);
            return dto with
            {
                CustomerGroupName = entity.CustomerGroupName
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
        return new GetPagedCustomerSubGroupResult
        {
            Data = new PagedList<GetPagedCustomerSubGroupDto>(dtos, totalRecords, (top > 0) ? (skip / top) + 1 : 1, top),
            Message = "Success"
        };
    }


}





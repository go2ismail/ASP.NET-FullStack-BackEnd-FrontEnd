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

namespace Application.Features.Configs.Queries;

public record GetPagedConfigDto
{
    public string Id { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public string CurrencyId { get; init; } = null!;
    public string? CurrencyName { get; init; }
    public string SmtpHost { get; init; } = null!;
    public int SmtpPort { get; init; }
    public string SmtpUserName { get; init; } = null!;
    public bool SmtpUseSSL { get; init; }
    public bool Active { get; init; }
}


public class GetPagedConfigProfile : Profile
{
    public GetPagedConfigProfile()
    {
        CreateMap<Config, GetPagedConfigDto>();
    }
}

public class GetPagedConfigResult
{
    public PagedList<GetPagedConfigDto>? Data { get; init; }
    public string Message { get; init; } = null!;
}

public class GetPagedConfigRequest : IRequest<GetPagedConfigResult>
{
    public IODataQueryOptions<Config> QueryOptions { get; init; } = null!;
    public string SearchValue { get; init; } = null!;
    public bool IsDeleted { get; init; } = false;
}

public class GetPagedConfigValidator : AbstractValidator<GetPagedConfigRequest>
{
    public GetPagedConfigValidator()
    {

        RuleFor(x => x.QueryOptions)
            .NotNull().WithMessage("Query options are required.");
    }
}



public class GetPagedConfigHandler(IMapper mapper, IQueryContext context) : IRequestHandler<GetPagedConfigRequest, GetPagedConfigResult>
{
    private readonly IMapper _mapper = mapper;
    private readonly IQueryContext _context = context;

    public async Task<GetPagedConfigResult> Handle(GetPagedConfigRequest request, CancellationToken cancellationToken)
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
        bool useDBSortingPaging = string.IsNullOrEmpty(orderby) || !orderby.Contains('.'); //dot meaning: relation property: entityName.propertyName

        var baseQuery = _context.Config
            .AsNoTracking()
            .ApplyIsDeletedFilter(request.IsDeleted)
            .AsQueryable();

        var configQuery = baseQuery;

        // Database-Side Filtering (Attributes)
        var odataQuery = baseQuery.ApplyODataFilter(request.QueryOptions);
        configQuery = odataQuery;

        // Combining Queries for Relationships
        if (!string.IsNullOrEmpty(keywords))
        {
            var currencyQuery = baseQuery.ApplyRelationFilter<Config, Currency>(keywords, "CurrencyId", _context);
            configQuery = odataQuery.Union(currencyQuery);
        }

        // Sorting & Paging Logic:
        if (useDBSortingPaging)
        {
            // Apply sorting and paging in the database if sorting is based on field properties
            configQuery = configQuery.ApplyODataSorting(request.QueryOptions);
            totalRecords = await configQuery.CountAsync(cancellationToken);
            configQuery = configQuery.ApplyODataPaging(request.QueryOptions, out skip, out top);
        }

        // Fetch Data & Map to DTOs
        var entities = await (
            from config in configQuery
            join currency in _context.Currency.AsNoTracking()
                on config.CurrencyId equals currency.Id into currencyLookup
            from currency in currencyLookup.DefaultIfEmpty()
            select new
            {
                config,
                CurrencyName = !string.IsNullOrWhiteSpace(currency.Name) ? currency.Name : string.Empty
            }
        ).ToListAsync(cancellationToken);

        var dtos = entities.Select(entity =>
        {
            var dto = _mapper.Map<GetPagedConfigDto>(entity.config);
            return dto with
            {
                CurrencyName = entity.CurrencyName
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
        return new GetPagedConfigResult
        {
            Data = new PagedList<GetPagedConfigDto>(dtos, totalRecords, (top > 0) ? (skip / top) + 1 : 1, top),
            Message = "Success"
        };
    }


}




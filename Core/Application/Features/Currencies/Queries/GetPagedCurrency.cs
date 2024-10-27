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

namespace Application.Features.Currencies.Queries;



public record GetPagedCurrencyDto
{
    public string Id { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string Symbol { get; init; } = null!;
    public string? Description { get; init; }
}


public class GetPagedCurrencyProfile : Profile
{
    public GetPagedCurrencyProfile()
    {
        CreateMap<Currency, GetPagedCurrencyDto>();
    }
}

public class GetPagedCurrencyResult
{
    public PagedList<GetPagedCurrencyDto>? Data { get; init; }
    public string Message { get; init; } = null!;
}

public class GetPagedCurrencyRequest : IRequest<GetPagedCurrencyResult>
{
    public IODataQueryOptions<Currency> QueryOptions { get; init; } = null!;
    public bool IsDeleted { get; init; } = false;
}

public class GetPagedCurrencyValidator : AbstractValidator<GetPagedCurrencyRequest>
{
    public GetPagedCurrencyValidator()
    {

        RuleFor(x => x.QueryOptions)
            .NotNull().WithMessage("Query options are required.");
    }
}


public class GetPagedCurrencyHandler : IRequestHandler<GetPagedCurrencyRequest, GetPagedCurrencyResult>
{
    private readonly IMapper _mapper;
    private readonly IQueryContext _context;

    public GetPagedCurrencyHandler(IMapper mapper, IQueryContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<GetPagedCurrencyResult> Handle(GetPagedCurrencyRequest request, CancellationToken cancellationToken)
    {
        int totalRecords = 0;
        int skip = 0;
        int top = 0;


        var query = _context
            .Currency
            .AsNoTracking()
            .ApplyIsDeletedFilter(request.IsDeleted)
            .AsQueryable();

        query = query
            .ApplyODataFilterWithPaging(request.QueryOptions, out totalRecords, out skip, out top);

        var entities = await query.ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<GetPagedCurrencyDto>>(entities);

        return new GetPagedCurrencyResult
        {
            Data = new PagedList<GetPagedCurrencyDto>(dtos, totalRecords, (skip / top) + 1, top),
            Message = "Success"
        };
    }


}




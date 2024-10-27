// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Common.Models;
using Application.Services.CQS.Queries;

using AutoMapper;

using Domain.Constants;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Application.Features.Currencies.Queries;



public class GetCurrencyLookupProfile : Profile
{
    public GetCurrencyLookupProfile()
    {
    }
}

public class GetCurrencyLookupResult
{
    public List<LookupDto> Data { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class GetCurrencyLookupRequest : IRequest<GetCurrencyLookupResult>
{
}

public class GetCurrencyLookupValidator : AbstractValidator<GetCurrencyLookupRequest>
{
    public GetCurrencyLookupValidator()
    {
    }
}


public class GetCurrencyLookupHandler : IRequestHandler<GetCurrencyLookupRequest, GetCurrencyLookupResult>
{
    private readonly IQueryContext _context;
    private readonly IMapper _mapper;

    public GetCurrencyLookupHandler(
        IQueryContext context,
        IMapper mapper
        )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GetCurrencyLookupResult> Handle(GetCurrencyLookupRequest request, CancellationToken cancellationToken)
    {
        var query = _context
            .Currency
            .AsNoTracking()
            .ApplyIsDeletedFilter()
            .AsQueryable();

        var entities = await query
            .Select(x => new LookupDto
            {
                Value = x.Id,
                Text = $"{x.Symbol} / {x.Name}"
            })
            .ToListAsync(cancellationToken);

        if (entities == null)
        {
            throw new ApplicationException($"{ExceptionConsts.EntitiyNotFound}");
        }

        var dto = entities;

        return new GetCurrencyLookupResult
        {
            Data = dto,
            Message = "Success"
        };
    }
}




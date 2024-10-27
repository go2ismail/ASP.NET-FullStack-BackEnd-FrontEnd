// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.CQS.Queries;

using AutoMapper;

using Domain.Constants;
using Domain.Entities;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Application.Features.Currencies.Queries;


public record GetCurrencyDto
{
    public string Id { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string Symbol { get; init; } = null!;
    public string? Description { get; init; }
}


public class GetCurrencyProfile : Profile
{
    public GetCurrencyProfile()
    {
        CreateMap<Currency, GetCurrencyDto>();
    }
}

public class GetCurrencyResult
{
    public GetCurrencyDto Data { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class GetCurrencyRequest : IRequest<GetCurrencyResult>
{
    public string Id { get; init; } = null!;
}

public class GetCurrencyValidator : AbstractValidator<GetCurrencyRequest>
{
    public GetCurrencyValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}


public class GetCurrencyHandler : IRequestHandler<GetCurrencyRequest, GetCurrencyResult>
{
    private readonly IQueryContext _context;
    private readonly IMapper _mapper;

    public GetCurrencyHandler(
        IQueryContext context,
        IMapper mapper
        )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GetCurrencyResult> Handle(GetCurrencyRequest request, CancellationToken cancellationToken)
    {
        var query = _context
            .Currency
            .AsNoTracking()
            .ApplyIsDeletedFilter()
            .AsQueryable();

        query = query
            .Where(x => x.Id == request.Id);

        var entity = await query.SingleOrDefaultAsync(cancellationToken);

        if (entity == null)
        {
            throw new ApplicationException($"{ExceptionConsts.EntitiyNotFound} {request.Id}");
        }

        var dto = _mapper.Map<GetCurrencyDto>(entity);

        return new GetCurrencyResult
        {
            Data = dto,
            Message = "Success"
        };
    }
}




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

namespace Application.Features.Configs.Queries;

public record GetActiveConfigDto
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


public class GetActiveConfigProfile : Profile
{
    public GetActiveConfigProfile()
    {
        CreateMap<Config, GetActiveConfigDto>();
    }
}

public class GetActiveConfigResult
{
    public GetActiveConfigDto Data { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class GetActiveConfigRequest : IRequest<GetActiveConfigResult>
{
}

public class GetActiveConfigValidator : AbstractValidator<GetActiveConfigRequest>
{
    public GetActiveConfigValidator()
    {
    }
}


public class GetActiveConfigHandler(
    IQueryContext context,
    IMapper mapper
        ) : IRequestHandler<GetActiveConfigRequest, GetActiveConfigResult>
{
    private readonly IQueryContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<GetActiveConfigResult> Handle(GetActiveConfigRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await (
            from config in _context.Config.AsNoTracking().ApplyIsDeletedFilter()
            join currency in _context.Currency.AsNoTracking().ApplyIsDeletedFilter()
                on config.CurrencyId equals currency.Id into currencyLookup
            from currency in currencyLookup.DefaultIfEmpty()
            where config.Active == true
            select new
            {
                config,
                CurrencyName = currency != null ? currency.Name : null
            }
            ).SingleOrDefaultAsync(cancellationToken) ?? throw new ApplicationException($"{ExceptionConsts.EntitiyNotFound}");
        var dto = _mapper.Map<GetActiveConfigDto>(entity.config);

        dto = dto with
        {
            CurrencyName = entity.CurrencyName
        };

        return new GetActiveConfigResult
        {
            Data = dto,
            Message = "Success"
        };
    }
}



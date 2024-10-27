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

public record GetConfigDto
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


public class GetConfigProfile : Profile
{
    public GetConfigProfile()
    {
        CreateMap<Config, GetConfigDto>();
    }
}

public class GetConfigResult
{
    public GetConfigDto Data { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class GetConfigRequest : IRequest<GetConfigResult>
{
    public string Id { get; init; } = null!;
}

public class GetConfigValidator : AbstractValidator<GetConfigRequest>
{
    public GetConfigValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}


public class GetConfigHandler(
    IQueryContext context,
    IMapper mapper
        ) : IRequestHandler<GetConfigRequest, GetConfigResult>
{
    private readonly IQueryContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<GetConfigResult> Handle(GetConfigRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await (
            from config in _context.Config.AsNoTracking().ApplyIsDeletedFilter()
            join currency in _context.Currency.AsNoTracking()
                on config.CurrencyId equals currency.Id into currencyLookup
            from currency in currencyLookup.DefaultIfEmpty()
            where config.Id == request.Id
            select new
            {
                config,
                CurrencyName = currency != null ? currency.Name : null
            }
            ).SingleOrDefaultAsync(cancellationToken) ?? throw new ApplicationException($"{ExceptionConsts.EntitiyNotFound} {request.Id}");
        var dto = _mapper.Map<GetConfigDto>(entity.config);

        dto = dto with
        {
            CurrencyName = entity.CurrencyName
        };

        return new GetConfigResult
        {
            Data = dto,
            Message = "Success"
        };
    }
}



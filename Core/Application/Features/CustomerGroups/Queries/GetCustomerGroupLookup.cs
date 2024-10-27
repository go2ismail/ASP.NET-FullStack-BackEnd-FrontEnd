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

namespace Application.Features.CustomerGroups.Queries;



public class GetCustomerGroupLookupProfile : Profile
{
    public GetCustomerGroupLookupProfile()
    {
    }
}

public class GetCustomerGroupLookupResult
{
    public List<LookupDto> Data { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class GetCustomerGroupLookupRequest : IRequest<GetCustomerGroupLookupResult>
{
}

public class GetCustomerGroupLookupValidator : AbstractValidator<GetCustomerGroupLookupRequest>
{
    public GetCustomerGroupLookupValidator()
    {
    }
}


public class GetCustomerGroupLookupHandler : IRequestHandler<GetCustomerGroupLookupRequest, GetCustomerGroupLookupResult>
{
    private readonly IQueryContext _context;
    private readonly IMapper _mapper;

    public GetCustomerGroupLookupHandler(
        IQueryContext context,
        IMapper mapper
        )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GetCustomerGroupLookupResult> Handle(GetCustomerGroupLookupRequest request, CancellationToken cancellationToken)
    {
        var query = _context
            .CustomerGroup
            .AsNoTracking()
            .ApplyIsDeletedFilter()
            .AsQueryable();

        var entities = await query
            .Select(x => new LookupDto
            {
                Value = x.Id,
                Text = $"{x.Name}"
            })
            .ToListAsync(cancellationToken);

        if (entities == null)
        {
            throw new ApplicationException($"{ExceptionConsts.EntitiyNotFound}");
        }

        var dto = entities;

        return new GetCustomerGroupLookupResult
        {
            Data = dto,
            Message = "Success"
        };
    }
}




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

namespace Application.Features.Customers.Queries;



public class GetCustomerLookupProfile : Profile
{
    public GetCustomerLookupProfile()
    {
    }
}

public class GetCustomerLookupResult
{
    public List<LookupDto> Data { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class GetCustomerLookupRequest : IRequest<GetCustomerLookupResult>
{
}

public class GetCustomerLookupValidator : AbstractValidator<GetCustomerLookupRequest>
{
    public GetCustomerLookupValidator()
    {
    }
}


public class GetCustomerLookupHandler : IRequestHandler<GetCustomerLookupRequest, GetCustomerLookupResult>
{
    private readonly IQueryContext _context;
    private readonly IMapper _mapper;

    public GetCustomerLookupHandler(
        IQueryContext context,
        IMapper mapper
        )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GetCustomerLookupResult> Handle(GetCustomerLookupRequest request, CancellationToken cancellationToken)
    {
        var query = _context
            .Customer
            .AsNoTracking()
            .ApplyIsDeletedFilter()
            .AsQueryable();

        var entities = await query
            .Select(x => new LookupDto
            {
                Value = x.Id,
                Text = $"{x.Code} / {x.Name}"
            })
            .ToListAsync(cancellationToken);

        if (entities == null)
        {
            throw new ApplicationException($"{ExceptionConsts.EntitiyNotFound}");
        }

        var dto = entities;

        return new GetCustomerLookupResult
        {
            Data = dto,
            Message = "Success"
        };
    }
}




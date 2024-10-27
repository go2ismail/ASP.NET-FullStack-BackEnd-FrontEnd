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

namespace Application.Features.CustomerSubGroups.Queries;



public class GetCustomerSubGroupLookupProfile : Profile
{
    public GetCustomerSubGroupLookupProfile()
    {
    }
}

public class GetCustomerSubGroupLookupResult
{
    public List<LookupDto> Data { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class GetCustomerSubGroupLookupRequest : IRequest<GetCustomerSubGroupLookupResult>
{
    public string? CustomerGroupId { get; init; }
}

public class GetCustomerSubGroupLookupValidator : AbstractValidator<GetCustomerSubGroupLookupRequest>
{
    public GetCustomerSubGroupLookupValidator()
    {
    }
}


public class GetCustomerSubGroupLookupHandler : IRequestHandler<GetCustomerSubGroupLookupRequest, GetCustomerSubGroupLookupResult>
{
    private readonly IQueryContext _context;
    private readonly IMapper _mapper;

    public GetCustomerSubGroupLookupHandler(
        IQueryContext context,
        IMapper mapper
        )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GetCustomerSubGroupLookupResult> Handle(GetCustomerSubGroupLookupRequest request, CancellationToken cancellationToken)
    {
        var query = _context
            .CustomerSubGroup
            .AsNoTracking()
            .ApplyIsDeletedFilter()
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.CustomerGroupId))
        {
            query = query.Where(x => x.CustomerGroupId == request.CustomerGroupId);
        }

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

        return new GetCustomerSubGroupLookupResult
        {
            Data = dto,
            Message = "Success"
        };
    }
}




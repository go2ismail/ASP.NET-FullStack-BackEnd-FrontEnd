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

namespace Application.Features.VendorGroups.Queries;



public class GetVendorGroupLookupProfile : Profile
{
    public GetVendorGroupLookupProfile()
    {
    }
}

public class GetVendorGroupLookupResult
{
    public List<LookupDto> Data { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class GetVendorGroupLookupRequest : IRequest<GetVendorGroupLookupResult>
{
}

public class GetVendorGroupLookupValidator : AbstractValidator<GetVendorGroupLookupRequest>
{
    public GetVendorGroupLookupValidator()
    {
    }
}


public class GetVendorGroupLookupHandler : IRequestHandler<GetVendorGroupLookupRequest, GetVendorGroupLookupResult>
{
    private readonly IQueryContext _context;
    private readonly IMapper _mapper;

    public GetVendorGroupLookupHandler(
        IQueryContext context,
        IMapper mapper
        )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GetVendorGroupLookupResult> Handle(GetVendorGroupLookupRequest request, CancellationToken cancellationToken)
    {
        var query = _context
            .VendorGroup
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

        return new GetVendorGroupLookupResult
        {
            Data = dto,
            Message = "Success"
        };
    }
}




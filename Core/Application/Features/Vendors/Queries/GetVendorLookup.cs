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

namespace Application.Features.Vendors.Queries;



public class GetVendorLookupProfile : Profile
{
    public GetVendorLookupProfile()
    {
    }
}

public class GetVendorLookupResult
{
    public List<LookupDto> Data { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class GetVendorLookupRequest : IRequest<GetVendorLookupResult>
{
}

public class GetVendorLookupValidator : AbstractValidator<GetVendorLookupRequest>
{
    public GetVendorLookupValidator()
    {
    }
}


public class GetVendorLookupHandler : IRequestHandler<GetVendorLookupRequest, GetVendorLookupResult>
{
    private readonly IQueryContext _context;
    private readonly IMapper _mapper;

    public GetVendorLookupHandler(
        IQueryContext context,
        IMapper mapper
        )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GetVendorLookupResult> Handle(GetVendorLookupRequest request, CancellationToken cancellationToken)
    {
        var query = _context
            .Vendor
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

        return new GetVendorLookupResult
        {
            Data = dto,
            Message = "Success"
        };
    }
}




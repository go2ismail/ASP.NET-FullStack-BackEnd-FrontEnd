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

namespace Application.Features.VendorSubGroups.Queries;



public class GetVendorSubGroupLookupProfile : Profile
{
    public GetVendorSubGroupLookupProfile()
    {
    }
}

public class GetVendorSubGroupLookupResult
{
    public List<LookupDto> Data { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class GetVendorSubGroupLookupRequest : IRequest<GetVendorSubGroupLookupResult>
{
    public string? VendorGroupId { get; init; }
}

public class GetVendorSubGroupLookupValidator : AbstractValidator<GetVendorSubGroupLookupRequest>
{
    public GetVendorSubGroupLookupValidator()
    {
    }
}


public class GetVendorSubGroupLookupHandler : IRequestHandler<GetVendorSubGroupLookupRequest, GetVendorSubGroupLookupResult>
{
    private readonly IQueryContext _context;
    private readonly IMapper _mapper;

    public GetVendorSubGroupLookupHandler(
        IQueryContext context,
        IMapper mapper
        )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GetVendorSubGroupLookupResult> Handle(GetVendorSubGroupLookupRequest request, CancellationToken cancellationToken)
    {
        var query = _context
            .VendorSubGroup
            .AsNoTracking()
            .ApplyIsDeletedFilter()
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.VendorGroupId))
        {
            query = query.Where(x => x.VendorGroupId == request.VendorGroupId);
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

        return new GetVendorSubGroupLookupResult
        {
            Data = dto,
            Message = "Success"
        };
    }
}




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

using System.Collections.ObjectModel;

namespace Application.Features.Vendors.Queries;


public record GetVendorDto
{
    public string Id { get; init; } = null!;
    public string Code { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public string VendorGroupId { get; init; } = null!;
    public string? VendorSubGroupId { get; init; }
    public string Street { get; init; } = null!;
    public string City { get; init; } = null!;
    public string StateOrProvince { get; init; } = null!;
    public string ZipCode { get; init; } = null!;
    public string? Country { get; init; }
    public string Phone { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string? Website { get; init; }
    public ICollection<VendorContact> VendorContacts { get; set; } = new Collection<VendorContact>();
}


public class GetVendorProfile : Profile
{
    public GetVendorProfile()
    {
        CreateMap<Vendor, GetVendorDto>();
    }
}

public class GetVendorResult
{
    public GetVendorDto Data { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class GetVendorRequest : IRequest<GetVendorResult>
{
    public string Id { get; init; } = null!;
}

public class GetVendorValidator : AbstractValidator<GetVendorRequest>
{
    public GetVendorValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}


public class GetVendorHandler : IRequestHandler<GetVendorRequest, GetVendorResult>
{
    private readonly IQueryContext _context;
    private readonly IMapper _mapper;

    public GetVendorHandler(
        IQueryContext context,
        IMapper mapper
        )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GetVendorResult> Handle(GetVendorRequest request, CancellationToken cancellationToken)
    {
        var query = _context
            .Vendor
            .AsNoTracking()
            .ApplyIsDeletedFilter()
            .AsQueryable();

        query = query
            .Where(x => x.Id == request.Id)
            .Include(x => x.VendorContacts);

        var entity = await query.SingleOrDefaultAsync(cancellationToken);

        if (entity == null)
        {
            throw new ApplicationException($"{ExceptionConsts.EntitiyNotFound} {request.Id}");
        }

        var dto = _mapper.Map<GetVendorDto>(entity);

        return new GetVendorResult
        {
            Data = dto,
            Message = "Success"
        };
    }
}





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

namespace Application.Features.VendorContacts.Queries;


public record GetVendorContactDto
{
    public string Id { get; init; } = null!;
    public string VendorId { get; init; } = null!;
    public string? VendorName { get; init; }
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string GenderId { get; init; } = null!;
    public string? GenderName { get; init; }
    public string? Description { get; init; }
    public string JobTitle { get; init; } = null!;
    public string? MobilePhone { get; init; }
    public string? SocialMedia { get; init; }
    public string? Address { get; init; }
    public string? City { get; init; }
    public string? StateOrProvince { get; init; }
    public string? ZipCode { get; init; }
    public string? Country { get; init; }
    public string Phone { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string? Website { get; init; }
}


public class GetVendorContactProfile : Profile
{
    public GetVendorContactProfile()
    {
        CreateMap<VendorContact, GetVendorContactDto>();
    }
}

public class GetVendorContactResult
{
    public GetVendorContactDto Data { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class GetVendorContactRequest : IRequest<GetVendorContactResult>
{
    public string Id { get; init; } = null!;
}

public class GetVendorContactValidator : AbstractValidator<GetVendorContactRequest>
{
    public GetVendorContactValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}


public class GetVendorContactHandler : IRequestHandler<GetVendorContactRequest, GetVendorContactResult>
{
    private readonly IQueryContext _context;
    private readonly IMapper _mapper;

    public GetVendorContactHandler(
        IQueryContext context,
        IMapper mapper
        )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GetVendorContactResult> Handle(GetVendorContactRequest request, CancellationToken cancellationToken)
    {
        var entity = await (
            from vendorContact in _context.VendorContact.AsNoTracking().ApplyIsDeletedFilter()
            join gender in _context.Gender.AsNoTracking()
                on vendorContact.GenderId equals gender.Id into genderLookup
            from gender in genderLookup.DefaultIfEmpty()
            where vendorContact.Id == request.Id
            select new
            {
                vendorContact,
                GenderName = gender != null ? gender.Name : null
            }
            ).SingleOrDefaultAsync(cancellationToken);

        if (entity == null)
        {
            throw new ApplicationException($"{ExceptionConsts.EntitiyNotFound} {request.Id}");
        }

        var dto = _mapper.Map<GetVendorContactDto>(entity);

        dto = dto with
        {
            GenderName = entity.GenderName
        };

        return new GetVendorContactResult
        {
            Data = dto,
            Message = "Success"
        };
    }
}






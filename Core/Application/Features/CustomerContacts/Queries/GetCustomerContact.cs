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

namespace Application.Features.CustomerContacts.Queries;


public record GetCustomerContactDto
{
    public string Id { get; init; } = null!;
    public string CustomerId { get; init; } = null!;
    public string? CustomerName { get; init; }
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


public class GetCustomerContactProfile : Profile
{
    public GetCustomerContactProfile()
    {
        CreateMap<CustomerContact, GetCustomerContactDto>();
    }
}

public class GetCustomerContactResult
{
    public GetCustomerContactDto Data { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class GetCustomerContactRequest : IRequest<GetCustomerContactResult>
{
    public string Id { get; init; } = null!;
}

public class GetCustomerContactValidator : AbstractValidator<GetCustomerContactRequest>
{
    public GetCustomerContactValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}


public class GetCustomerContactHandler : IRequestHandler<GetCustomerContactRequest, GetCustomerContactResult>
{
    private readonly IQueryContext _context;
    private readonly IMapper _mapper;

    public GetCustomerContactHandler(
        IQueryContext context,
        IMapper mapper
        )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GetCustomerContactResult> Handle(GetCustomerContactRequest request, CancellationToken cancellationToken)
    {
        var entity = await (
            from customerContact in _context.CustomerContact.AsNoTracking().ApplyIsDeletedFilter()
            join gender in _context.Gender.AsNoTracking()
                on customerContact.GenderId equals gender.Id into genderLookup
            from gender in genderLookup.DefaultIfEmpty()
            where customerContact.Id == request.Id
            select new
            {
                customerContact,
                GenderName = gender != null ? gender.Name : null
            }
            ).SingleOrDefaultAsync(cancellationToken);

        if (entity == null)
        {
            throw new ApplicationException($"{ExceptionConsts.EntitiyNotFound} {request.Id}");
        }

        var dto = _mapper.Map<GetCustomerContactDto>(entity);

        dto = dto with
        {
            GenderName = entity.GenderName
        };

        return new GetCustomerContactResult
        {
            Data = dto,
            Message = "Success"
        };
    }
}





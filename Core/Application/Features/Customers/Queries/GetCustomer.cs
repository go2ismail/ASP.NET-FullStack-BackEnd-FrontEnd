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

namespace Application.Features.Customers.Queries;

public record GetCustomerDto
{
    public string Id { get; init; } = null!;
    public string Code { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public string CustomerGroupId { get; init; } = null!;
    public string? CustomerSubGroupId { get; init; }
    public string Street { get; init; } = null!;
    public string City { get; init; } = null!;
    public string StateOrProvince { get; init; } = null!;
    public string ZipCode { get; init; } = null!;
    public string? Country { get; init; }
    public string Phone { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string? Website { get; init; }
    public ICollection<CustomerContact> CustomerContacts { get; set; } = new Collection<CustomerContact>();
}


public class GetCustomerProfile : Profile
{
    public GetCustomerProfile()
    {
        CreateMap<Customer, GetCustomerDto>();
    }
}

public class GetCustomerResult
{
    public GetCustomerDto Data { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class GetCustomerRequest : IRequest<GetCustomerResult>
{
    public string Id { get; init; } = null!;
}

public class GetCustomerValidator : AbstractValidator<GetCustomerRequest>
{
    public GetCustomerValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}


public class GetCustomerHandler(
    IQueryContext context,
    IMapper mapper
        ) : IRequestHandler<GetCustomerRequest, GetCustomerResult>
{
    private readonly IQueryContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<GetCustomerResult> Handle(GetCustomerRequest request, CancellationToken cancellationToken)
    {
        var query = _context
            .Customer
            .AsNoTracking()
            .ApplyIsDeletedFilter()
            .AsQueryable();

        query = query
            .Where(x => x.Id == request.Id)
            .Include(x => x.CustomerContacts);

        var entity = await query.SingleOrDefaultAsync(cancellationToken);

        if (entity == null)
        {
            throw new ApplicationException($"{ExceptionConsts.EntitiyNotFound} {request.Id}");
        }

        var dto = _mapper.Map<GetCustomerDto>(entity);

        return new GetCustomerResult
        {
            Data = dto,
            Message = "Success"
        };
    }
}




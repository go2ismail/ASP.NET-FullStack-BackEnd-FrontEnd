// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.CQS.Queries;

using AutoMapper;

using Domain.Entities;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Application.Features.CustomerSubGroups.Queries;



public record GetCustomerSubGroupDto
{
    public string Id { get; init; } = null!;
    public string CustomerGroupId { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
}


public class GetCustomerSubGroupProfile : Profile
{
    public GetCustomerSubGroupProfile()
    {
        CreateMap<CustomerSubGroup, GetCustomerSubGroupDto>();
    }
}

public class GetCustomerSubGroupResult
{
    public GetCustomerSubGroupDto Data { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class GetCustomerSubGroupRequest : IRequest<GetCustomerSubGroupResult>
{
    public string Id { get; init; } = null!;
}

public class GetCustomerSubGroupValidator : AbstractValidator<GetCustomerSubGroupRequest>
{
    public GetCustomerSubGroupValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}


public class GetCustomerSubGroupHandler : IRequestHandler<GetCustomerSubGroupRequest, GetCustomerSubGroupResult>
{
    private readonly IMapper _mapper;
    private readonly IQueryContext _context;

    public GetCustomerSubGroupHandler(
        IMapper mapper,
        IQueryContext context
        )
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<GetCustomerSubGroupResult> Handle(GetCustomerSubGroupRequest request, CancellationToken cancellationToken)
    {
        var query = _context
            .CustomerSubGroup
            .AsNoTracking()
            .ApplyIsDeletedFilter()
            .AsQueryable();

        query = query
            .Where(x => x.Id == request.Id);

        var entity = await query.SingleAsync(cancellationToken);

        if (entity == null)
        {
            throw new ApplicationException($"Entity with Id {request.Id} not found.");
        }

        var dto = _mapper.Map<GetCustomerSubGroupDto>(entity);

        return new GetCustomerSubGroupResult
        {
            Data = dto,
            Message = "Success"
        };
    }
}





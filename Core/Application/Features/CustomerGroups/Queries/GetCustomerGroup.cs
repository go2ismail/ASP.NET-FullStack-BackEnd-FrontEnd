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

namespace Application.Features.CustomerGroups.Queries;



public record GetCustomerGroupDto
{
    public string Id { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
}


public class GetCustomerGroupProfile : Profile
{
    public GetCustomerGroupProfile()
    {
        CreateMap<CustomerGroup, GetCustomerGroupDto>();
    }
}

public class GetCustomerGroupResult
{
    public GetCustomerGroupDto Data { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class GetCustomerGroupRequest : IRequest<GetCustomerGroupResult>
{
    public string Id { get; init; } = null!;
}

public class GetCustomerGroupValidator : AbstractValidator<GetCustomerGroupRequest>
{
    public GetCustomerGroupValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}


public class GetCustomerGroupHandler : IRequestHandler<GetCustomerGroupRequest, GetCustomerGroupResult>
{
    private readonly IQueryContext _context;
    private readonly IMapper _mapper;

    public GetCustomerGroupHandler(
        IQueryContext context,
        IMapper mapper
        )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GetCustomerGroupResult> Handle(GetCustomerGroupRequest request, CancellationToken cancellationToken)
    {
        var query = _context
            .CustomerGroup
            .AsNoTracking()
            .ApplyIsDeletedFilter()
            .AsQueryable();

        query = query
            .Where(x => x.Id == request.Id);

        var entity = await query.SingleOrDefaultAsync(cancellationToken);

        if (entity == null)
        {
            throw new ApplicationException($"{ExceptionConsts.EntitiyNotFound} {request.Id}");
        }

        var dto = _mapper.Map<GetCustomerGroupDto>(entity);

        return new GetCustomerGroupResult
        {
            Data = dto,
            Message = "Success"
        };
    }
}




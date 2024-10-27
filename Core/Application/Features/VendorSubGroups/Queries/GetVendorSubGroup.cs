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

namespace Application.Features.VendorSubGroups.Queries;



public record GetVendorSubGroupDto
{
    public string Id { get; init; } = null!;
    public string VendorGroupId { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
}


public class GetVendorSubGroupProfile : Profile
{
    public GetVendorSubGroupProfile()
    {
        CreateMap<VendorSubGroup, GetVendorSubGroupDto>();
    }
}

public class GetVendorSubGroupResult
{
    public GetVendorSubGroupDto Data { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class GetVendorSubGroupRequest : IRequest<GetVendorSubGroupResult>
{
    public string Id { get; init; } = null!;
}

public class GetVendorSubGroupValidator : AbstractValidator<GetVendorSubGroupRequest>
{
    public GetVendorSubGroupValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}


public class GetVendorSubGroupHandler : IRequestHandler<GetVendorSubGroupRequest, GetVendorSubGroupResult>
{
    private readonly IMapper _mapper;
    private readonly IQueryContext _context;

    public GetVendorSubGroupHandler(
        IMapper mapper,
        IQueryContext context
        )
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<GetVendorSubGroupResult> Handle(GetVendorSubGroupRequest request, CancellationToken cancellationToken)
    {
        var query = _context
            .VendorSubGroup
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

        var dto = _mapper.Map<GetVendorSubGroupDto>(entity);

        return new GetVendorSubGroupResult
        {
            Data = dto,
            Message = "Success"
        };
    }
}







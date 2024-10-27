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

namespace Application.Features.VendorGroups.Queries;



public record GetVendorGroupDto
{
    public string Id { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
}


public class GetVendorGroupProfile : Profile
{
    public GetVendorGroupProfile()
    {
        CreateMap<VendorGroup, GetVendorGroupDto>();
    }
}

public class GetVendorGroupResult
{
    public GetVendorGroupDto Data { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class GetVendorGroupRequest : IRequest<GetVendorGroupResult>
{
    public string Id { get; init; } = null!;
}

public class GetVendorGroupValidator : AbstractValidator<GetVendorGroupRequest>
{
    public GetVendorGroupValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}


public class GetVendorGroupHandler : IRequestHandler<GetVendorGroupRequest, GetVendorGroupResult>
{
    private readonly IQueryContext _context;
    private readonly IMapper _mapper;

    public GetVendorGroupHandler(
        IQueryContext context,
        IMapper mapper
        )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GetVendorGroupResult> Handle(GetVendorGroupRequest request, CancellationToken cancellationToken)
    {
        var query = _context
            .VendorGroup
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

        var dto = _mapper.Map<GetVendorGroupDto>(entity);

        return new GetVendorGroupResult
        {
            Data = dto,
            Message = "Success"
        };
    }
}





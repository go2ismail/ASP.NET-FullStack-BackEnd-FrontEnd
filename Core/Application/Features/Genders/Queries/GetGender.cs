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

namespace Application.Features.Genders.Queries;


public record GetGenderDto
{
    public string Id { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string Symbol { get; init; } = null!;
    public string? Description { get; init; }
}


public class GetGenderProfile : Profile
{
    public GetGenderProfile()
    {
        CreateMap<Gender, GetGenderDto>();
    }
}

public class GetGenderResult
{
    public GetGenderDto Data { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class GetGenderRequest : IRequest<GetGenderResult>
{
    public string Id { get; init; } = null!;
}

public class GetGenderValidator : AbstractValidator<GetGenderRequest>
{
    public GetGenderValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}


public class GetGenderHandler : IRequestHandler<GetGenderRequest, GetGenderResult>
{
    private readonly IQueryContext _context;
    private readonly IMapper _mapper;

    public GetGenderHandler(
        IQueryContext context,
        IMapper mapper
        )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GetGenderResult> Handle(GetGenderRequest request, CancellationToken cancellationToken)
    {
        var query = _context
            .Gender
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

        var dto = _mapper.Map<GetGenderDto>(entity);

        return new GetGenderResult
        {
            Data = dto,
            Message = "Success"
        };
    }
}




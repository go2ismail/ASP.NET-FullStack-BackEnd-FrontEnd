// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Common.Models;
using Application.Services.CQS.Queries;

using AutoMapper;

using Domain.Entities;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Application.Features.Genders.Queries;



public record GetPagedGenderDto
{
    public string Id { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
}


public class GetPagedGenderProfile : Profile
{
    public GetPagedGenderProfile()
    {
        CreateMap<Gender, GetPagedGenderDto>();
    }
}

public class GetPagedGenderResult
{
    public PagedList<GetPagedGenderDto>? Data { get; init; }
    public string Message { get; init; } = null!;
}

public class GetPagedGenderRequest : IRequest<GetPagedGenderResult>
{
    public IODataQueryOptions<Gender> QueryOptions { get; init; } = null!;
    public bool IsDeleted { get; init; } = false;
}

public class GetPagedGenderValidator : AbstractValidator<GetPagedGenderRequest>
{
    public GetPagedGenderValidator()
    {

        RuleFor(x => x.QueryOptions)
            .NotNull().WithMessage("Query options are required.");
    }
}


public class GetPagedGenderHandler : IRequestHandler<GetPagedGenderRequest, GetPagedGenderResult>
{
    private readonly IMapper _mapper;
    private readonly IQueryContext _context;

    public GetPagedGenderHandler(IMapper mapper, IQueryContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<GetPagedGenderResult> Handle(GetPagedGenderRequest request, CancellationToken cancellationToken)
    {
        int totalRecords = 0;
        int skip = 0;
        int top = 0;


        var query = _context
            .Gender
            .AsNoTracking()
            .ApplyIsDeletedFilter(request.IsDeleted)
            .AsQueryable();

        query = query
            .ApplyODataFilterWithPaging(request.QueryOptions, out totalRecords, out skip, out top);

        var entities = await query.ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<GetPagedGenderDto>>(entities);

        return new GetPagedGenderResult
        {
            Data = new PagedList<GetPagedGenderDto>(dtos, totalRecords, (skip / top) + 1, top),
            Message = "Success"
        };
    }


}




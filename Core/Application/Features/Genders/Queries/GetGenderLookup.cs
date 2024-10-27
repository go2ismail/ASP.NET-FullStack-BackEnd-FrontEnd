// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Common.Models;
using Application.Services.CQS.Queries;

using AutoMapper;

using Domain.Constants;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Application.Features.Genders.Queries;



public class GetGenderLookupProfile : Profile
{
    public GetGenderLookupProfile()
    {
    }
}

public class GetGenderLookupResult
{
    public List<LookupDto> Data { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class GetGenderLookupRequest : IRequest<GetGenderLookupResult>
{
}

public class GetGenderLookupValidator : AbstractValidator<GetGenderLookupRequest>
{
    public GetGenderLookupValidator()
    {
    }
}


public class GetGenderLookupHandler : IRequestHandler<GetGenderLookupRequest, GetGenderLookupResult>
{
    private readonly IQueryContext _context;
    private readonly IMapper _mapper;

    public GetGenderLookupHandler(
        IQueryContext context,
        IMapper mapper
        )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GetGenderLookupResult> Handle(GetGenderLookupRequest request, CancellationToken cancellationToken)
    {
        var query = _context
            .Gender
            .AsNoTracking()
            .ApplyIsDeletedFilter()
            .AsQueryable();

        var entities = await query
            .Select(x => new LookupDto
            {
                Value = x.Id,
                Text = $"{x.Name}"
            })
            .ToListAsync(cancellationToken);

        if (entities == null)
        {
            throw new ApplicationException($"{ExceptionConsts.EntitiyNotFound}");
        }

        var dto = entities;

        return new GetGenderLookupResult
        {
            Data = dto,
            Message = "Success"
        };
    }
}




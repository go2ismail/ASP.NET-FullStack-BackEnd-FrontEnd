// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Common.Models;
using Application.Services.Externals;

using AutoMapper;

using Domain.Constants;

using FluentValidation;

using MediatR;

namespace Application.Features.Accounts.Queries;



public class GetClaimLookupProfile : Profile
{
    public GetClaimLookupProfile()
    {
    }
}

public class GetClaimLookupResult
{
    public List<LookupDto> Data { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class GetClaimLookupRequest : IRequest<GetClaimLookupResult>
{
}

public class GetClaimLookupValidator : AbstractValidator<GetClaimLookupRequest>
{
    public GetClaimLookupValidator()
    {
    }
}


public class GetClaimLookupHandler : IRequestHandler<GetClaimLookupRequest, GetClaimLookupResult>
{
    private readonly IIdentityService _identityService;
    private readonly IMapper _mapper;

    public GetClaimLookupHandler(
        IIdentityService identityService,
        IMapper mapper
        )
    {
        _identityService = identityService;
        _mapper = mapper;
    }

    public async Task<GetClaimLookupResult> Handle(GetClaimLookupRequest request, CancellationToken cancellationToken)
    {

        var entities = await _identityService.GetClaimLookupAsync(cancellationToken);

        if (entities == null)
        {
            throw new ApplicationException($"{ExceptionConsts.EntitiyNotFound}");
        }

        var dto = entities;

        return new GetClaimLookupResult
        {
            Data = dto,
            Message = "Success"
        };
    }
}




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



public class GetRoleLookupProfile : Profile
{
    public GetRoleLookupProfile()
    {
    }
}

public class GetRoleLookupResult
{
    public List<LookupDto> Data { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class GetRoleLookupRequest : IRequest<GetRoleLookupResult>
{
}

public class GetRoleLookupValidator : AbstractValidator<GetRoleLookupRequest>
{
    public GetRoleLookupValidator()
    {
    }
}


public class GetRoleLookupHandler : IRequestHandler<GetRoleLookupRequest, GetRoleLookupResult>
{
    private readonly IIdentityService _identityService;
    private readonly IMapper _mapper;

    public GetRoleLookupHandler(
        IIdentityService identityService,
        IMapper mapper
        )
    {
        _identityService = identityService;
        _mapper = mapper;
    }

    public async Task<GetRoleLookupResult> Handle(GetRoleLookupRequest request, CancellationToken cancellationToken)
    {

        var entities = await _identityService.GetRoleLookupAsync(cancellationToken);

        if (entities == null)
        {
            throw new ApplicationException($"{ExceptionConsts.EntitiyNotFound}");
        }


        return entities;
    }
}




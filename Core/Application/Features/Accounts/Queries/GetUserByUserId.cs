// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Features.Accounts.Dtos;
using Application.Services.Externals;

using FluentValidation;

using MediatR;

namespace Application.Features.Accounts.Queries;


public class GetUserByUserIdResult
{
    public ApplicationUserDto? User { get; init; }
}

public class GetUserByUserIdRequest : IRequest<GetUserByUserIdResult>
{
    public required string UserId { get; init; }
}

public class GetUserByUserIdValidator : AbstractValidator<GetUserByUserIdRequest>
{
    public GetUserByUserIdValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}


public class GetUserByUserIdHandler : IRequestHandler<GetUserByUserIdRequest, GetUserByUserIdResult>
{
    private readonly IIdentityService _identityService;

    public GetUserByUserIdHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<GetUserByUserIdResult> Handle(GetUserByUserIdRequest request, CancellationToken cancellationToken)
    {
        var result = await _identityService.GetUserByUserIdAsync(
            request.UserId,
            cancellationToken);

        return result;
    }
}




// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Features.Accounts.Dtos;
using Application.Services.Externals;

using FluentValidation;

using MediatR;

namespace Application.Features.Accounts.Queries;


public class GetUsersByUserIdResult
{
    public List<ApplicationUserDto> Users { get; init; } = null!;
}

public class GetUsersByUserIdRequest : IRequest<GetUsersByUserIdResult>
{
    public required string UserId { get; init; }
}

public class GetUsersByUserIdValidator : AbstractValidator<GetUsersByUserIdRequest>
{
    public GetUsersByUserIdValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}


public class GetUsersByUserIdHandler : IRequestHandler<GetUsersByUserIdRequest, GetUsersByUserIdResult>
{
    private readonly IIdentityService _identityService;

    public GetUsersByUserIdHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<GetUsersByUserIdResult> Handle(GetUsersByUserIdRequest request, CancellationToken cancellationToken)
    {
        var result = await _identityService.GetUsersByUserIdAsync(
            request.UserId,
            cancellationToken);

        return result;
    }
}




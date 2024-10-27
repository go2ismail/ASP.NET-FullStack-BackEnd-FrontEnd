// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.Externals;

using FluentValidation;

using MediatR;

namespace Application.Features.Accounts.Commands;

public class LogoutUserResult
{
    public string? UserId { get; init; }
}

public class LogoutUserRequest : IRequest<LogoutUserResult>
{
    public required string UserId { get; init; }
}

public class LogoutUserValidator : AbstractValidator<LogoutUserResult>
{
    public LogoutUserValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
public class LogoutUserHandler(IIdentityService identityService) : IRequestHandler<LogoutUserRequest, LogoutUserResult>
{
    private readonly IIdentityService _identityService = identityService;

    public async Task<LogoutUserResult> Handle(LogoutUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _identityService.LogoutAsync(request.UserId, cancellationToken);

        return result;
    }
}
// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Features.NavigationManagers.Queries;
using Application.Services.Externals;

using FluentValidation;

using MediatR;

namespace Application.Features.Accounts.Commands;

public class LoginUserResult
{
    public string? AccessToken { get; init; }
    public string? RefreshToken { get; init; }
    public string? UserId { get; init; }
    public string? Email { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public List<string>? UserClaims { get; init; }
    public List<MainNavDto>? MainNavigations { get; init; }
}

public class LoginUserRequest : IRequest<LoginUserResult>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}

public class LoginUserValidator : AbstractValidator<LoginUserRequest>
{
    public LoginUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}

public class LoginUserHandler(IIdentityService identityService) : IRequestHandler<LoginUserRequest, LoginUserResult>
{
    private readonly IIdentityService _identityService = identityService;

    public async Task<LoginUserResult> Handle(LoginUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _identityService.LoginAsync(
            request.Email,
            request.Password,
            cancellationToken
            );

        return result;
    }
}

// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.Externals;

using FluentValidation;

using MediatR;

namespace Application.Features.Accounts.Commands;


public class CreateRoleResult
{
    public string? Role { get; init; }
    public string[]? Claims { get; init; }
}

public class CreateRoleRequest : IRequest<CreateRoleResult>
{
    public required string Role { get; init; }
    public required string[] Claims { get; init; }
}

public class CreateRoleValidator : AbstractValidator<CreateRoleRequest>
{
    public CreateRoleValidator()
    {
        RuleFor(x => x.Role)
            .NotEmpty();

        RuleFor(x => x.Claims)
            .NotNull();
    }
}


public class CreateRoleHandler : IRequestHandler<CreateRoleRequest, CreateRoleResult>
{
    private readonly IIdentityService _identityService;

    public CreateRoleHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<CreateRoleResult> Handle(CreateRoleRequest request, CancellationToken cancellationToken)
    {
        var result = await _identityService.CreateRoleAsync(
            request.Role,
            request.Claims,
            cancellationToken
            );

        return result;
    }
}


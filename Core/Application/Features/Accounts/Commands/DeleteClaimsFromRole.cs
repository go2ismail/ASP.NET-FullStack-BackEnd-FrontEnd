// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.Externals;

using FluentValidation;

using MediatR;

namespace Application.Features.Accounts.Commands;


public class DeleteClaimsFromRoleResult
{
    public string? Role { get; init; }
    public string[] Claims { get; init; } = Array.Empty<string>();
}

public class DeleteClaimsFromRoleRequest : IRequest<DeleteClaimsFromRoleResult>
{
    public required string Role { get; init; }
    public string[] Claims { get; init; } = Array.Empty<string>();
}

public class DeleteClaimsFromRoleValidator : AbstractValidator<DeleteClaimsFromRoleRequest>
{
    public DeleteClaimsFromRoleValidator()
    {
        RuleFor(x => x.Role)
            .NotEmpty();

        RuleFor(x => x.Claims)
            .NotEmpty();
    }
}

public class DeleteClaimsFromRoleHandler : IRequestHandler<DeleteClaimsFromRoleRequest, DeleteClaimsFromRoleResult>
{
    private readonly IIdentityService _identityService;

    public DeleteClaimsFromRoleHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<DeleteClaimsFromRoleResult> Handle(DeleteClaimsFromRoleRequest request, CancellationToken cancellationToken)
    {
        var result = await _identityService.DeleteClaimsFromRoleAsync(
            request.Role,
            request.Claims,
            cancellationToken
            );

        return result;
    }
}




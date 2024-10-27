// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.Externals;

using FluentValidation;

using MediatR;

namespace Application.Features.Accounts.Commands;



public class UpdateRoleResult
{
    public string? Role { get; init; }
    public string[]? Claims { get; init; }
}

public class UpdateRoleRequest : IRequest<UpdateRoleResult>
{
    public required string OldRole { get; init; }
    public required string NewRole { get; init; }
    public required string[] Claims { get; init; }
}

public class UpdateRoleValidator : AbstractValidator<UpdateRoleRequest>
{
    public UpdateRoleValidator()
    {
        RuleFor(x => x.OldRole)
            .NotEmpty();

        RuleFor(x => x.NewRole)
            .NotEmpty();

        RuleFor(x => x.Claims)
            .NotNull();
    }
}


public class UpdateRoleHandler(IIdentityService identityService) : IRequestHandler<UpdateRoleRequest, UpdateRoleResult>
{
    private readonly IIdentityService _identityService = identityService;

    public async Task<UpdateRoleResult> Handle(UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        var result = await _identityService.UpdateRoleAsync(
            request.OldRole,
            request.NewRole,
            request.Claims,
            cancellationToken
            );

        return result;
    }
}




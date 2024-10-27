// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.Externals;

using FluentValidation;

using MediatR;

namespace Application.Features.Accounts.Commands;



public class DeleteRoleResult
{
}

public class DeleteRoleRequest : IRequest<DeleteRoleResult>
{
    public required string Role { get; init; }
}

public class DeleteRoleValidator : AbstractValidator<DeleteRoleRequest>
{
    public DeleteRoleValidator()
    {
        RuleFor(x => x.Role)
            .NotEmpty();
    }
}


public class DeleteRoleHandler : IRequestHandler<DeleteRoleRequest, DeleteRoleResult>
{
    private readonly IIdentityService _identityService;

    public DeleteRoleHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<DeleteRoleResult> Handle(DeleteRoleRequest request, CancellationToken cancellationToken)
    {
        var result = await _identityService.DeleteRoleAsync(
            request.Role,
            cancellationToken
            );

        return result;
    }
}



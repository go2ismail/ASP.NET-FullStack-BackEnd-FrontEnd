// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.Externals;

using FluentValidation;

using MediatR;

namespace Application.Features.Accounts.Commands;

public class DeleteUserResult
{
    public string? Id { get; init; }
    public string? Email { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
}

public class DeleteUserRequest : IRequest<DeleteUserResult>
{
    public required string UserId { get; init; }
}

public class DeleteUserValidator : AbstractValidator<DeleteUserRequest>
{
    public DeleteUserValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}

public class DeleteUserHandler : IRequestHandler<DeleteUserRequest, DeleteUserResult>
{
    private readonly IIdentityService _identityService;

    public DeleteUserHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<DeleteUserResult> Handle(DeleteUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _identityService.DeleteUserAsync(request.UserId, cancellationToken);

        return result;
    }
}

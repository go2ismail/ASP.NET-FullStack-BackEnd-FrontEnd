// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.Externals;

using FluentValidation;

using MediatR;

namespace Application.Features.Members.Commands;

public class DeleteMemberResult
{
    public string? Id { get; init; }
    public string Email { get; init; } = null!;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
}

public class DeleteMemberRequest : IRequest<DeleteMemberResult>
{
    public required string Email { get; init; }
}

public class DeleteMemberValidator : AbstractValidator<DeleteMemberRequest>
{
    public DeleteMemberValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty();
    }
}


public class DeleteMemberHandler : IRequestHandler<DeleteMemberRequest, DeleteMemberResult>
{
    private readonly IMediator _mediator;
    private readonly IIdentityService _identityService;

    public DeleteMemberHandler(
        IMediator mediator,
        IIdentityService identityService
        )
    {
        _identityService = identityService;
        _mediator = mediator;
    }

    public async Task<DeleteMemberResult> Handle(DeleteMemberRequest request, CancellationToken cancellationToken)
    {
        var result = await _identityService.DeleteMemberAsync(
            request.Email,
            cancellationToken
            );

        return result;
    }
}


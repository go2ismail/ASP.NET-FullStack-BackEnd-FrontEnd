// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.Externals;

using FluentValidation;

using MediatR;

namespace Application.Features.Members.Commands;

public class UpdateMemberResult
{
    public string? Id { get; init; }
    public string Email { get; init; } = null!;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
}

public class UpdateMemberRequest : IRequest<UpdateMemberResult>
{
    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string? Password { get; init; }
    public string? ConfirmPassword { get; init; }
    public required bool EmailConfirmed { get; init; }
    public required bool IsBlocked { get; init; }
    public required bool IsDeleted { get; init; }
    public string[]? Roles { get; init; }
}

public class UpdateMemberValidator : AbstractValidator<UpdateMemberRequest>
{
    public UpdateMemberValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty();

        RuleFor(x => x.FirstName)
            .NotEmpty();

        RuleFor(x => x.LastName)
            .NotEmpty();

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("Should equal with Password.");

        RuleFor(x => x.Roles)
            .NotNull();
    }
}


public class UpdateMemberHandler : IRequestHandler<UpdateMemberRequest, UpdateMemberResult>
{
    private readonly IMediator _mediator;
    private readonly IIdentityService _identityService;

    public UpdateMemberHandler(
        IMediator mediator,
        IIdentityService identityService
        )
    {
        _identityService = identityService;
        _mediator = mediator;
    }

    public async Task<UpdateMemberResult> Handle(UpdateMemberRequest request, CancellationToken cancellationToken)
    {
        var result = await _identityService.UpdateMemberAsync(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            request.EmailConfirmed,
            request.IsBlocked,
            request.IsDeleted,
            request.Roles,
            cancellationToken
            );

        return result;
    }
}


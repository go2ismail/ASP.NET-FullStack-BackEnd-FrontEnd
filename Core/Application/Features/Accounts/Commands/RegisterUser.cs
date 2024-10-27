// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Features.Accounts.Events;
using Application.Services.Externals;

using FluentValidation;

using MediatR;

namespace Application.Features.Accounts.Commands;

public class RegisterUserResult
{
    public string? Id { get; init; }
    public string Email { get; init; } = null!;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? EmailConfirmationToken { get; init; }
    public bool SendEmailConfirmation { get; init; }
    public string Host { get; init; } = null!;
}

public class RegisterUserRequest : IRequest<RegisterUserResult>
{
    public required string Email { get; init; }
    public required string Host { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Password { get; init; }
    public required string ConfirmPassword { get; init; }
}

public class RegisterUserValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty();

        RuleFor(x => x.Host)
            .NotEmpty();

        RuleFor(x => x.FirstName)
            .NotEmpty();

        RuleFor(x => x.LastName)
            .NotEmpty();

        RuleFor(x => x.Password)
            .NotEmpty();

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .Equal(x => x.Password).WithMessage("Password and Confirm Password should equal.");
    }
}


public class RegisterUserHandler(
    IMediator mediator,
    IIdentityService identityService
        ) : IRequestHandler<RegisterUserRequest, RegisterUserResult>
{
    private readonly IMediator _mediator = mediator;
    private readonly IIdentityService _identityService = identityService;

    public async Task<RegisterUserResult> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _identityService.RegisterUserAsync(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            cancellationToken
            );

        var registerUserEvent = new RegisterUserEvent
        (
            result.Email,
            result.FirstName,
            result.LastName,
            result.EmailConfirmationToken,
            result.SendEmailConfirmation,
            request.Host
        );
        await _mediator.Publish(registerUserEvent, cancellationToken);

        return result;
    }
}


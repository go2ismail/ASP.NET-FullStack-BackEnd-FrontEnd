// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.Externals;

using FluentValidation;

using MediatR;

namespace Application.Features.Accounts.Commands;

public class CreateUserResult
{
    public string? Id { get; init; }
    public string? Email { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
}

public class CreateUserRequest : IRequest<CreateUserResult>
{
    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string CreatedById { get; init; }
    public required string Password { get; init; }
    public required string ConfirmPassword { get; init; }
}

public class CreateUserValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.FirstName)
            .NotEmpty();

        RuleFor(x => x.LastName)
            .NotEmpty();

        RuleFor(x => x.CreatedById)
            .NotEmpty();

        RuleFor(x => x.Password)
            .NotEmpty();

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .Equal(x => x.Password).WithMessage("Password and Confirm Password should equal.");
    }
}


public class CreateUserHandler : IRequestHandler<CreateUserRequest, CreateUserResult>
{
    private readonly IIdentityService _identityService;

    public CreateUserHandler(
        IIdentityService identityService
        )
    {
        _identityService = identityService;
    }

    public async Task<CreateUserResult> Handle(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _identityService.CreateUserAsync(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            request.CreatedById,
            cancellationToken
            );

        return result;
    }
}

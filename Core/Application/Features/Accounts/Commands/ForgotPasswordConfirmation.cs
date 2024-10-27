// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.Externals;

using FluentValidation;

using MediatR;

namespace Application.Features.Accounts.Commands;

public class ForgotPasswordConfirmationResult
{
    public string? Email { get; init; }
}

public class ForgotPasswordConfirmationRequest : IRequest<ForgotPasswordConfirmationResult>
{
    public required string Email { get; init; }
    public required string TempPassword { get; init; }
    public required string Code { get; init; }
}

public class ForgotPasswordConfirmationValidator : AbstractValidator<ForgotPasswordConfirmationRequest>
{
    public ForgotPasswordConfirmationValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.TempPassword)
            .NotEmpty();

        RuleFor(x => x.Code)
            .NotEmpty();
    }
}


public class ForgotPasswordConfirmationHandler : IRequestHandler<ForgotPasswordConfirmationRequest, ForgotPasswordConfirmationResult>
{
    private readonly IIdentityService _identityService;

    public ForgotPasswordConfirmationHandler(
        IIdentityService identityService
        )
    {
        _identityService = identityService;
    }

    public async Task<ForgotPasswordConfirmationResult> Handle(ForgotPasswordConfirmationRequest request, CancellationToken cancellationToken)
    {
        var result = await _identityService.ForgotPasswordConfirmationAsync(
            request.Email,
            request.TempPassword,
            request.Code,
            cancellationToken
            );

        return new ForgotPasswordConfirmationResult { Email = result };
    }
}

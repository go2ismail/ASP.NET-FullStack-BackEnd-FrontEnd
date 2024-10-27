// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Common.Models;
using Application.Services.Externals;

using FluentValidation;

using MediatR;

namespace Application.Features.Accounts.Queries;


public class GetRolesByUserResult
{
    public PagedList<string> Roles { get; init; } = null!;
}

public class GetRolesByUserRequest : IRequest<GetRolesByUserResult>
{
    public required string UserId { get; init; }
    public required int Page { get; init; }
    public required int Limit { get; init; }
}

public class GetRolesByUserValidator : AbstractValidator<GetRolesByUserRequest>
{
    public GetRolesByUserValidator()
    {
        RuleFor(x => x.Page)
            .NotEmpty();

        RuleFor(x => x.Limit)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}


public class GetRolesByUserHandler : IRequestHandler<GetRolesByUserRequest, GetRolesByUserResult>
{
    private readonly IIdentityService _identityService;

    public GetRolesByUserHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<GetRolesByUserResult> Handle(GetRolesByUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _identityService.GetRolesByUserAsync(
            request.UserId,
            request.Page,
            request.Limit,
            cancellationToken
            );

        return result;
    }
}


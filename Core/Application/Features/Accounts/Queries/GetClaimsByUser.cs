// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Common.Models;
using Application.Features.Accounts.Dtos;
using Application.Services.Externals;

using FluentValidation;

using MediatR;

namespace Application.Features.Accounts.Queries;

public class GetClaimsByUserResult
{
    public PagedList<ClaimDto> Claims { get; init; } = null!;
}

public class GetClaimsByUserRequest : IRequest<GetClaimsByUserResult>
{
    public required string UserId { get; init; }
    public required int Page { get; init; }
    public required int Limit { get; init; }
    public required string SortBy { get; init; }
    public required string SortDirection { get; init; }
    public string searchValue { get; init; } = string.Empty;
}

public class GetClaimsByUserValidator : AbstractValidator<GetClaimsByUserRequest>
{
    public GetClaimsByUserValidator()
    {
        RuleFor(x => x.Page)
            .NotEmpty();

        RuleFor(x => x.Limit)
            .NotEmpty();

        RuleFor(x => x.SortBy)
            .NotEmpty();

        RuleFor(x => x.SortDirection)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}


public class GetClaimsByUserHandler : IRequestHandler<GetClaimsByUserRequest, GetClaimsByUserResult>
{
    private readonly IIdentityService _identityService;

    public GetClaimsByUserHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<GetClaimsByUserResult> Handle(GetClaimsByUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _identityService.GetClaimsByUserAsync(
            request.UserId,
            request.Page,
            request.Limit,
            request.SortBy,
            request.SortDirection,
            request.searchValue,
            cancellationToken
            );

        return result;
    }
}



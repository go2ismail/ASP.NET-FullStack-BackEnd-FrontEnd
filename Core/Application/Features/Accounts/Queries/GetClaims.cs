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



public class GetClaimsResult
{
    public PagedList<ClaimDto>? Data { get; init; } = null!;
    public string Message { get; init; } = null!;
}

public class GetClaimsRequest : IRequest<GetClaimsResult>
{
    public required int Page { get; init; }
    public required int Limit { get; init; }
    public required string SortBy { get; init; }
    public required string SortDirection { get; init; }
    public string searchValue { get; init; } = string.Empty;
}

public class GetClaimsValidator : AbstractValidator<GetClaimsRequest>
{
    public GetClaimsValidator()
    {
        RuleFor(x => x.Page)
            .NotEmpty();

        RuleFor(x => x.Limit)
            .NotEmpty();

        RuleFor(x => x.SortBy)
            .NotEmpty();

        RuleFor(x => x.SortDirection)
            .NotEmpty();
    }
}


public class GetClaimsHandler : IRequestHandler<GetClaimsRequest, GetClaimsResult>
{
    private readonly IIdentityService _identityService;

    public GetClaimsHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<GetClaimsResult> Handle(GetClaimsRequest request, CancellationToken cancellationToken)
    {
        var result = await _identityService.GetClaimsAsync(
            request.Page,
            request.Limit,
            request.SortBy,
            request.SortDirection,
            request.searchValue,
            cancellationToken
            );

        return new GetClaimsResult
        {
            Data = result.Data,
            Message = "Success"
        };
    }
}



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


public class GetUsersResult
{
    public PagedList<ApplicationUserDto>? Data { get; init; }
    public string Message { get; init; } = null!;
}

public class GetUsersRequest : IRequest<GetUsersResult>
{
    public required int Page { get; init; }
    public required int Limit { get; init; }
    public required string SortBy { get; init; }
    public required string SortDirection { get; init; }
    public string searchValue { get; init; } = string.Empty;
}

public class GetUsersValidator : AbstractValidator<GetUsersRequest>
{
    public GetUsersValidator()
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


public class GetUsersHandler : IRequestHandler<GetUsersRequest, GetUsersResult>
{
    private readonly IIdentityService _identityService;

    public GetUsersHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<GetUsersResult> Handle(GetUsersRequest request, CancellationToken cancellationToken)
    {
        var result = await _identityService.GetUsersAsync(
            request.Page,
            request.Limit,
            request.SortBy,
            request.SortDirection,
            request.searchValue,
            cancellationToken);

        return new GetUsersResult
        {
            Data = result.Data,
            Message = "Success"
        };
    }
}




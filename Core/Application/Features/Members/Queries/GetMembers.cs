// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Common.Models;
using Application.Features.Accounts.Dtos;
using Application.Services.Externals;

using FluentValidation;

using MediatR;

namespace Application.Features.Members.Queries;


public class GetMembersResult
{
    public PagedList<ApplicationUserDto>? Data { get; init; }
    public string Message { get; init; } = null!;
}

public class GetMembersRequest : IRequest<GetMembersResult>
{
    public required int Page { get; init; }
    public required int Limit { get; init; }
    public required string SortBy { get; init; }
    public required string SortDirection { get; init; }
    public string searchValue { get; init; } = string.Empty;
}

public class GetMembersValidator : AbstractValidator<GetMembersRequest>
{
    public GetMembersValidator()
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


public class GetMembersHandler : IRequestHandler<GetMembersRequest, GetMembersResult>
{
    private readonly IIdentityService _identityService;

    public GetMembersHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<GetMembersResult> Handle(GetMembersRequest request, CancellationToken cancellationToken)
    {
        var result = await _identityService.GetUsersAsync(
            request.Page,
            request.Limit,
            request.SortBy,
            request.SortDirection,
            request.searchValue,
            cancellationToken);

        return new GetMembersResult
        {
            Data = result.Data,
            Message = "Success"
        };
    }
}




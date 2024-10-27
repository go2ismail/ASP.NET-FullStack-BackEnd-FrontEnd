// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Features.Accounts.Commands;
using Application.Features.Accounts.Queries;

using global::WebAPI.Common.Filters;
using global::WebAPI.Common.Models;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.Claims;


[Route("api/[controller]")]
public class ClaimController(ISender sender) : BaseApiController(sender)
{
    [ClaimBasedAuthorization("Update")]
    [HttpPost("AddClaimsToRole")]
    public async Task<ActionResult<ApiSuccessResult<AddClaimsToRoleResult>>> AddClaimsToRoleAsync(AddClaimsToRoleRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<AddClaimsToRoleResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(AddClaimsToRoleAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Update")]
    [HttpDelete("DeleteClaimsFromRole")]
    public async Task<ActionResult<ApiSuccessResult<DeleteClaimsFromRoleResult>>> DeleteClaimsFromRoleAsync(DeleteClaimsFromRoleRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<DeleteClaimsFromRoleResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(DeleteClaimsFromRoleAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Read")]
    [HttpGet("GetClaimsByUser")]
    public async Task<ActionResult<ApiSuccessResult<GetClaimsByUserResult>>> GetClaimsByUserAsync(
        [FromQuery(Name = "$skip")] int skip,
        [FromQuery(Name = "$top")] int top,
        [FromQuery(Name = "$orderby")] string orderBy,
        [FromQuery] string searchValue,
        [FromQuery] string userId,
        CancellationToken cancellationToken)
    {
        int page = (skip / top) + 1;
        int limit = top;

        var orderByParts = orderBy.Split(' ');
        var sortBy = orderByParts[0];
        var sortDirection = orderByParts.Length > 1 ? orderByParts[1].ToLower() : "asc";

        var command = new GetClaimsByUserRequest
        {
            Page = page,
            Limit = limit,
            SortBy = sortBy,
            SortDirection = sortDirection,
            searchValue = searchValue,
            UserId = userId
        };
        var response = await _sender.Send(command, cancellationToken);

        return Ok(new ApiSuccessResult<GetClaimsByUserResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetClaimsByUserAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Read")]
    [HttpGet("GetClaims")]
    public async Task<ActionResult<ApiSuccessResult<GetClaimsResult>>> GetClaimsAsync(
        [FromQuery(Name = "$skip")] int skip,
        [FromQuery(Name = "$top")] int top,
        [FromQuery(Name = "$orderby")] string orderBy,
        [FromQuery] string searchValue,
        [FromQuery] string userId,
        CancellationToken cancellationToken)
    {
        int page = (skip / top) + 1;
        int limit = top;

        var orderByParts = orderBy.Split(' ');
        var sortBy = orderByParts[0];
        var sortDirection = orderByParts.Length > 1 ? orderByParts[1].ToLower() : "asc";

        var command = new GetClaimsRequest
        {
            Page = page,
            Limit = limit,
            SortBy = sortBy,
            SortDirection = sortDirection,
            searchValue = searchValue
        };
        var response = await _sender.Send(command, cancellationToken);

        return Ok(new ApiSuccessResult<GetClaimsResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetClaimsByUserAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Read")]
    [HttpGet("GetClaimsByRole")]
    public async Task<ActionResult<ApiSuccessResult<GetClaimsByRoleResult>>> GetClaimsByRoleAsync(
        [FromQuery(Name = "$skip")] int skip,
        [FromQuery(Name = "$top")] int top,
        [FromQuery] string role,
        CancellationToken cancellationToken)
    {
        int page = (skip / top) + 1;
        int limit = top;

        var command = new GetClaimsByRoleRequest { Page = page, Limit = limit, Role = role };
        var response = await _sender.Send(command, cancellationToken);

        return Ok(new ApiSuccessResult<GetClaimsByRoleResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetClaimsByRoleAsync)}",
            Content = response
        });
    }



    [Authorize]
    [HttpGet("GetClaimLookup")]
    public async Task<ActionResult<ApiSuccessResult<GetClaimLookupResult>>> GetClaimLookupAsync(
        CancellationToken cancellationToken)
    {
        var request = new GetClaimLookupRequest();
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<GetClaimLookupResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetClaimLookupAsync)}",
            Content = response
        });
    }

}


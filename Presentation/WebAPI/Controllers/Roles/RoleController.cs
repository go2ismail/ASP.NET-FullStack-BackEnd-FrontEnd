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

namespace WebAPI.Controllers.Roles;

[Route("api/[controller]")]
public class RoleController : BaseApiController
{
    public RoleController(ISender sender) : base(sender) { }

    [ClaimBasedAuthorization("Create")]
    [HttpPost("CreateRole")]
    public async Task<ActionResult<ApiSuccessResult<CreateRoleResult>>> CreateRoleAsync(CreateRoleRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<CreateRoleResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(CreateRoleAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Delete")]
    [HttpDelete("DeleteRole")]
    public async Task<ActionResult<ApiSuccessResult<DeleteRoleResult>>> DeleteRoleAsync(DeleteRoleRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<DeleteRoleResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(DeleteRoleAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Update")]
    [HttpPost("UpdateRole")]
    public async Task<ActionResult<ApiSuccessResult<UpdateRoleResult>>> UpdateRoleAsync(UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<UpdateRoleResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(UpdateRoleAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Read")]
    [HttpGet("GetRoles")]
    public async Task<ActionResult<ApiSuccessResult<GetRolesResult>>> GetRolesAsync(
        [FromQuery(Name = "$skip")] int skip,
        [FromQuery(Name = "$top")] int top,
        [FromQuery(Name = "$orderby")] string orderBy,
        [FromQuery] string searchValue,
        CancellationToken cancellationToken)
    {
        int page = (skip / top) + 1;
        int limit = top;

        var orderByParts = orderBy.Split(' ');
        var sortBy = orderByParts[0];
        var sortDirection = orderByParts.Length > 1 ? orderByParts[1].ToLower() : "asc";

        var command = new GetRolesRequest
        {
            Page = page,
            Limit = limit,
            SortBy = sortBy,
            SortDirection = sortDirection,
            searchValue = searchValue,
        };
        var response = await _sender.Send(command, cancellationToken);

        return Ok(new ApiSuccessResult<GetRolesResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetRolesAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Read")]
    [HttpGet("GetRolesByUser")]
    public async Task<ActionResult<ApiSuccessResult<GetRolesByUserResult>>> GetRolesByUserAsync(
        [FromQuery(Name = "$skip")] int skip,
        [FromQuery(Name = "$top")] int top,
        [FromQuery(Name = "$orderby")] string orderBy,
        [FromQuery] string userId,
        CancellationToken cancellationToken)
    {
        int page = (skip / top) + 1;
        int limit = top;

        var command = new GetRolesByUserRequest { Page = page, Limit = limit, UserId = userId };
        var response = await _sender.Send(command, cancellationToken); ;

        return Ok(new ApiSuccessResult<GetRolesByUserResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetRolesAsync)}",
            Content = response
        });
    }



    [Authorize]
    [HttpGet("GetRoleLookup")]
    public async Task<ActionResult<ApiSuccessResult<GetRoleLookupResult>>> GetRoleLookupAsync(
        CancellationToken cancellationToken)
    {
        var request = new GetRoleLookupRequest();
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<GetRoleLookupResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetRoleLookupAsync)}",
            Content = response
        });
    }

}


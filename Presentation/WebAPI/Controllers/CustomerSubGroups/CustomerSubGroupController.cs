// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Features.CustomerSubGroups.Commands;
using Application.Features.CustomerSubGroups.Queries;

using Domain.Entities;

using Infrastructure.DataAccessManagers.EFCores.ODatas;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

using WebAPI.Common.Filters;
using WebAPI.Common.Models;

namespace WebAPI.Controllers.CustomerSubGroups;

[Route("api/[controller]")]
public class CustomerSubGroupController(ISender sender) : BaseApiController(sender)
{
    [ClaimBasedAuthorization("Create")]
    [HttpPost("CreateCustomerSubGroup")]
    public async Task<ActionResult<ApiSuccessResult<CreateCustomerSubGroupResult>>> CreateCustomerSubGroupAsync(CreateCustomerSubGroupRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<CreateCustomerSubGroupResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(CreateCustomerSubGroupAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Update")]
    [HttpPost("UpdateCustomerSubGroup")]
    public async Task<ActionResult<ApiSuccessResult<UpdateCustomerSubGroupResult>>> UpdateCustomerSubGroupAsync(UpdateCustomerSubGroupRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<UpdateCustomerSubGroupResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(UpdateCustomerSubGroupAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Delete")]
    [HttpDelete("DeleteCustomerSubGroup")]
    public async Task<ActionResult<ApiSuccessResult<DeleteCustomerSubGroupResult>>> DeleteCustomerSubGroupAsync(DeleteCustomerSubGroupRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<DeleteCustomerSubGroupResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(DeleteCustomerSubGroupAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Read")]
    [HttpGet("GetPagedCustomerSubGroup")]
    public async Task<ActionResult<ApiSuccessResult<GetPagedCustomerSubGroupResult>>> GetPagedCustomerSubGroupAsync(
        ODataQueryOptions<CustomerSubGroup> options,
        CancellationToken cancellationToken,
        [FromQuery] string searchValue,
        [FromQuery] bool isDeleted = false)
    {
        var queryOptionsAdapter = new ODataQueryOptionsAdapter<CustomerSubGroup>(options);
        var request = new GetPagedCustomerSubGroupRequest
        {
            QueryOptions = queryOptionsAdapter,
            SearchValue = searchValue,
            IsDeleted = isDeleted
        };
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<GetPagedCustomerSubGroupResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetPagedCustomerSubGroupAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Read")]
    [HttpGet("GetCustomerSubGroup")]
    public async Task<ActionResult<ApiSuccessResult<GetCustomerSubGroupResult>>> GetCustomerSubGroupAsync([FromQuery] string id, CancellationToken cancellationToken)
    {
        var request = new GetCustomerSubGroupRequest { Id = id };
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<GetCustomerSubGroupResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetCustomerSubGroupAsync)}",
            Content = response
        });
    }

    [Authorize]
    [HttpGet("GetCustomerSubGroupLookup")]
    public async Task<ActionResult<ApiSuccessResult<GetCustomerSubGroupLookupResult>>> GetCustomerSubGroupLookupAsync(
        [FromQuery] string? customerGroupId,
        CancellationToken cancellationToken)
    {
        var request = new GetCustomerSubGroupLookupRequest
        {
            CustomerGroupId = customerGroupId
        };
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<GetCustomerSubGroupLookupResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetCustomerSubGroupLookupAsync)}",
            Content = response
        });
    }


}


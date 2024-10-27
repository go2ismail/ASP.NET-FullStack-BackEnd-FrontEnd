// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Features.CustomerGroups.Commands;
using Application.Features.CustomerGroups.Queries;

using Domain.Entities;

using Infrastructure.DataAccessManagers.EFCores.ODatas;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

using WebAPI.Common.Filters;
using WebAPI.Common.Models;

namespace WebAPI.Controllers.CustomerGroups;

[Route("api/[controller]")]
public class CustomerGroupController : BaseApiController
{
    public CustomerGroupController(ISender sender) : base(sender)
    {
    }

    [ClaimBasedAuthorization("Create")]
    [HttpPost("CreateCustomerGroup")]
    public async Task<ActionResult<ApiSuccessResult<CreateCustomerGroupResult>>> CreateCustomerGroupAsync(CreateCustomerGroupRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<CreateCustomerGroupResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(CreateCustomerGroupAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Update")]
    [HttpPost("UpdateCustomerGroup")]
    public async Task<ActionResult<ApiSuccessResult<UpdateCustomerGroupResult>>> UpdateCustomerGroupAsync(UpdateCustomerGroupRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<UpdateCustomerGroupResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(UpdateCustomerGroupAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Delete")]
    [HttpDelete("DeleteCustomerGroup")]
    public async Task<ActionResult<ApiSuccessResult<DeleteCustomerGroupResult>>> DeleteCustomerGroupAsync(DeleteCustomerGroupRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<DeleteCustomerGroupResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(DeleteCustomerGroupAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Read")]
    [HttpGet("GetPagedCustomerGroup")]
    public async Task<ActionResult<ApiSuccessResult<GetPagedCustomerGroupResult>>> GetPagedCustomerGroupAsync(
        ODataQueryOptions<CustomerGroup> options,
        CancellationToken cancellationToken,
        [FromQuery] string searchValue,
        [FromQuery] bool isDeleted = false)
    {
        var queryOptionsAdapter = new ODataQueryOptionsAdapter<CustomerGroup>(options);
        var request = new GetPagedCustomerGroupRequest
        {
            QueryOptions = queryOptionsAdapter,
            SearchValue = searchValue,
            IsDeleted = isDeleted
        };
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<GetPagedCustomerGroupResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetPagedCustomerGroupAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Read")]
    [HttpGet("GetCustomerGroup")]
    public async Task<ActionResult<ApiSuccessResult<GetCustomerGroupResult>>> GetCustomerGroupAsync([FromQuery] string id, CancellationToken cancellationToken)
    {
        var request = new GetCustomerGroupRequest { Id = id };
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<GetCustomerGroupResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetCustomerGroupAsync)}",
            Content = response
        });
    }

    [Authorize]
    [HttpGet("GetCustomerGroupLookup")]
    public async Task<ActionResult<ApiSuccessResult<GetCustomerGroupLookupResult>>> GetCustomerGroupLookupAsync(
        CancellationToken cancellationToken)
    {
        var request = new GetCustomerGroupLookupRequest();
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<GetCustomerGroupLookupResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetCustomerGroupLookupAsync)}",
            Content = response
        });
    }


}

// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Features.VendorSubGroups.Commands;
using Application.Features.VendorSubGroups.Queries;

using Domain.Entities;

using Infrastructure.DataAccessManagers.EFCores.ODatas;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

using WebAPI.Common.Filters;
using WebAPI.Common.Models;

namespace WebAPI.Controllers.VendorSubGroups;

[Route("api/[controller]")]
public class VendorSubGroupController : BaseApiController
{
    public VendorSubGroupController(ISender sender) : base(sender)
    {
    }

    [ClaimBasedAuthorization("Create")]
    [HttpPost("CreateVendorSubGroup")]
    public async Task<ActionResult<ApiSuccessResult<CreateVendorSubGroupResult>>> CreateVendorSubGroupAsync(CreateVendorSubGroupRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<CreateVendorSubGroupResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(CreateVendorSubGroupAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Update")]
    [HttpPost("UpdateVendorSubGroup")]
    public async Task<ActionResult<ApiSuccessResult<UpdateVendorSubGroupResult>>> UpdateVendorSubGroupAsync(UpdateVendorSubGroupRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<UpdateVendorSubGroupResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(UpdateVendorSubGroupAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Delete")]
    [HttpDelete("DeleteVendorSubGroup")]
    public async Task<ActionResult<ApiSuccessResult<DeleteVendorSubGroupResult>>> DeleteVendorSubGroupAsync(DeleteVendorSubGroupRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<DeleteVendorSubGroupResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(DeleteVendorSubGroupAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Read")]
    [HttpGet("GetPagedVendorSubGroup")]
    public async Task<ActionResult<ApiSuccessResult<GetPagedVendorSubGroupResult>>> GetPagedVendorSubGroupAsync(
        ODataQueryOptions<VendorSubGroup> options,
        CancellationToken cancellationToken,
        [FromQuery] string searchValue,
        [FromQuery] bool isDeleted = false)
    {
        var queryOptionsAdapter = new ODataQueryOptionsAdapter<VendorSubGroup>(options);
        var request = new GetPagedVendorSubGroupRequest
        {
            QueryOptions = queryOptionsAdapter,
            SearchValue = searchValue,
            IsDeleted = isDeleted
        };
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<GetPagedVendorSubGroupResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetPagedVendorSubGroupAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Read")]
    [HttpGet("GetVendorSubGroup")]
    public async Task<ActionResult<ApiSuccessResult<GetVendorSubGroupResult>>> GetVendorSubGroupAsync([FromQuery] string id, CancellationToken cancellationToken)
    {
        var request = new GetVendorSubGroupRequest { Id = id };
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<GetVendorSubGroupResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetVendorSubGroupAsync)}",
            Content = response
        });
    }

    [Authorize]
    [HttpGet("GetVendorSubGroupLookup")]
    public async Task<ActionResult<ApiSuccessResult<GetVendorSubGroupLookupResult>>> GetVendorSubGroupLookupAsync(
        [FromQuery] string? vendorGroupId,
        CancellationToken cancellationToken)
    {
        var request = new GetVendorSubGroupLookupRequest
        {
            VendorGroupId = vendorGroupId
        };
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<GetVendorSubGroupLookupResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetVendorSubGroupLookupAsync)}",
            Content = response
        });
    }


}


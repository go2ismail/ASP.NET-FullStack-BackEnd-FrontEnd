// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Features.VendorGroups.Commands;
using Application.Features.VendorGroups.Queries;

using Domain.Entities;

using Infrastructure.DataAccessManagers.EFCores.ODatas;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

using WebAPI.Common.Filters;
using WebAPI.Common.Models;

namespace WebAPI.Controllers.VendorGroups;

[Route("api/[controller]")]
public class VendorGroupController : BaseApiController
{
    public VendorGroupController(ISender sender) : base(sender)
    {
    }

    [ClaimBasedAuthorization("Create")]
    [HttpPost("CreateVendorGroup")]
    public async Task<ActionResult<ApiSuccessResult<CreateVendorGroupResult>>> CreateVendorGroupAsync(CreateVendorGroupRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<CreateVendorGroupResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(CreateVendorGroupAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Update")]
    [HttpPost("UpdateVendorGroup")]
    public async Task<ActionResult<ApiSuccessResult<UpdateVendorGroupResult>>> UpdateVendorGroupAsync(UpdateVendorGroupRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<UpdateVendorGroupResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(UpdateVendorGroupAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Delete")]
    [HttpDelete("DeleteVendorGroup")]
    public async Task<ActionResult<ApiSuccessResult<DeleteVendorGroupResult>>> DeleteVendorGroupAsync(DeleteVendorGroupRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<DeleteVendorGroupResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(DeleteVendorGroupAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Read")]
    [HttpGet("GetPagedVendorGroup")]
    public async Task<ActionResult<ApiSuccessResult<GetPagedVendorGroupResult>>> GetPagedVendorGroupAsync(
        ODataQueryOptions<VendorGroup> options,
        CancellationToken cancellationToken,
        [FromQuery] string searchValue,
        [FromQuery] bool isDeleted = false)
    {
        var queryOptionsAdapter = new ODataQueryOptionsAdapter<VendorGroup>(options);
        var request = new GetPagedVendorGroupRequest
        {
            QueryOptions = queryOptionsAdapter,
            SearchValue = searchValue,
            IsDeleted = isDeleted
        };
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<GetPagedVendorGroupResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetPagedVendorGroupAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Read")]
    [HttpGet("GetVendorGroup")]
    public async Task<ActionResult<ApiSuccessResult<GetVendorGroupResult>>> GetVendorGroupAsync([FromQuery] string id, CancellationToken cancellationToken)
    {
        var request = new GetVendorGroupRequest { Id = id };
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<GetVendorGroupResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetVendorGroupAsync)}",
            Content = response
        });
    }

    [Authorize]
    [HttpGet("GetVendorGroupLookup")]
    public async Task<ActionResult<ApiSuccessResult<GetVendorGroupLookupResult>>> GetVendorGroupLookupAsync(
    CancellationToken cancellationToken)
    {
        var request = new GetVendorGroupLookupRequest();
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<GetVendorGroupLookupResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetVendorGroupLookupAsync)}",
            Content = response
        });
    }


}


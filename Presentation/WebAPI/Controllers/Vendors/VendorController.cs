// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Features.Vendors.Commands;
using Application.Features.Vendors.Queries;

using Domain.Entities;

using Infrastructure.DataAccessManagers.EFCores.ODatas;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

using WebAPI.Common.Filters;
using WebAPI.Common.Models;

namespace WebAPI.Controllers.Vendors;

[Route("api/[controller]")]
public class VendorController : BaseApiController
{
    public VendorController(ISender sender) : base(sender)
    {
    }

    [ClaimBasedAuthorization("Create")]
    [HttpPost("CreateVendor")]
    public async Task<ActionResult<ApiSuccessResult<CreateVendorResult>>> CreateVendorAsync(CreateVendorRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<CreateVendorResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(CreateVendorAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Update")]
    [HttpPost("UpdateVendor")]
    public async Task<ActionResult<ApiSuccessResult<UpdateVendorResult>>> UpdateVendorAsync(UpdateVendorRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<UpdateVendorResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(UpdateVendorAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Delete")]
    [HttpDelete("DeleteVendor")]
    public async Task<ActionResult<ApiSuccessResult<DeleteVendorResult>>> DeleteVendorAsync(DeleteVendorRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<DeleteVendorResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(DeleteVendorAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Read")]
    [HttpGet("GetPagedVendor")]
    public async Task<ActionResult<ApiSuccessResult<GetPagedVendorResult>>> GetPagedVendorAsync(
        ODataQueryOptions<Vendor> options,
        CancellationToken cancellationToken,
        [FromQuery] string searchValue,
        [FromQuery] bool isDeleted = false)
    {
        var queryOptionsAdapter = new ODataQueryOptionsAdapter<Vendor>(options);
        var request = new GetPagedVendorRequest
        {
            QueryOptions = queryOptionsAdapter,
            SearchValue = searchValue,
            IsDeleted = isDeleted
        };
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<GetPagedVendorResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetPagedVendorAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Read")]
    [HttpGet("GetVendor")]
    public async Task<ActionResult<ApiSuccessResult<GetVendorResult>>> GetVendorAsync([FromQuery] string id, CancellationToken cancellationToken)
    {
        var request = new GetVendorRequest { Id = id };
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<GetVendorResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetVendorAsync)}",
            Content = response
        });
    }


    [Authorize]
    [HttpGet("GetVendorLookup")]
    public async Task<ActionResult<ApiSuccessResult<GetVendorLookupResult>>> GetVendorLookupAsync(
        CancellationToken cancellationToken)
    {
        var request = new GetVendorLookupRequest();
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<GetVendorLookupResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetVendorLookupAsync)}",
            Content = response
        });
    }


}


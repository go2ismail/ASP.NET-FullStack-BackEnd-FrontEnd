// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Features.VendorContacts.Commands;
using Application.Features.VendorContacts.Queries;

using Domain.Entities;

using Infrastructure.DataAccessManagers.EFCores.ODatas;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

using WebAPI.Common.Filters;
using WebAPI.Common.Models;

namespace WebAPI.Controllers.VendorContacts;

[Route("api/[controller]")]
public class VendorContactController : BaseApiController
{
    public VendorContactController(ISender sender) : base(sender)
    {
    }

    [ClaimBasedAuthorization("Create")]
    [HttpPost("CreateVendorContact")]
    public async Task<ActionResult<ApiSuccessResult<CreateVendorContactResult>>> CreateVendorContactAsync(CreateVendorContactRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<CreateVendorContactResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(CreateVendorContactAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Update")]
    [HttpPost("UpdateVendorContact")]
    public async Task<ActionResult<ApiSuccessResult<UpdateVendorContactResult>>> UpdateVendorContactAsync(UpdateVendorContactRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<UpdateVendorContactResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(UpdateVendorContactAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Delete")]
    [HttpDelete("DeleteVendorContact")]
    public async Task<ActionResult<ApiSuccessResult<DeleteVendorContactResult>>> DeleteVendorContactAsync(DeleteVendorContactRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<DeleteVendorContactResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(DeleteVendorContactAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Read")]
    [HttpGet("GetPagedVendorContact")]
    public async Task<ActionResult<ApiSuccessResult<GetPagedVendorContactResult>>> GetPagedVendorContactAsync(
        ODataQueryOptions<VendorContact> options,
        CancellationToken cancellationToken,
        [FromQuery] string searchValue,
        [FromQuery] bool isDeleted = false)
    {
        var queryOptionsAdapter = new ODataQueryOptionsAdapter<VendorContact>(options);
        var request = new GetPagedVendorContactRequest
        {
            QueryOptions = queryOptionsAdapter,
            SearchValue = searchValue,
            IsDeleted = isDeleted
        };
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<GetPagedVendorContactResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetPagedVendorContactAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Read")]
    [HttpGet("GetVendorContact")]
    public async Task<ActionResult<ApiSuccessResult<GetVendorContactResult>>> GetVendorContactAsync([FromQuery] string id, CancellationToken cancellationToken)
    {
        var request = new GetVendorContactRequest { Id = id };
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<GetVendorContactResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetVendorContactAsync)}",
            Content = response
        });
    }

}


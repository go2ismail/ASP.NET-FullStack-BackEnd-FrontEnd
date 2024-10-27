// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Features.CustomerContacts.Commands;
using Application.Features.CustomerContacts.Queries;

using Domain.Entities;

using Infrastructure.DataAccessManagers.EFCores.ODatas;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

using WebAPI.Common.Filters;
using WebAPI.Common.Models;

namespace WebAPI.Controllers.CustomerContacts;

[Route("api/[controller]")]
public class CustomerContactController : BaseApiController
{
    public CustomerContactController(ISender sender) : base(sender)
    {
    }

    [ClaimBasedAuthorization("Create")]
    [HttpPost("CreateCustomerContact")]
    public async Task<ActionResult<ApiSuccessResult<CreateCustomerContactResult>>> CreateCustomerContactAsync(CreateCustomerContactRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<CreateCustomerContactResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(CreateCustomerContactAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Update")]
    [HttpPost("UpdateCustomerContact")]
    public async Task<ActionResult<ApiSuccessResult<UpdateCustomerContactResult>>> UpdateCustomerContactAsync(UpdateCustomerContactRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<UpdateCustomerContactResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(UpdateCustomerContactAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Delete")]
    [HttpDelete("DeleteCustomerContact")]
    public async Task<ActionResult<ApiSuccessResult<DeleteCustomerContactResult>>> DeleteCustomerContactAsync(DeleteCustomerContactRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<DeleteCustomerContactResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(DeleteCustomerContactAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Read")]
    [HttpGet("GetPagedCustomerContact")]
    public async Task<ActionResult<ApiSuccessResult<GetPagedCustomerContactResult>>> GetPagedCustomerContactAsync(
        ODataQueryOptions<CustomerContact> options,
        CancellationToken cancellationToken,
        [FromQuery] string searchValue,
        [FromQuery] bool isDeleted = false)
    {
        var queryOptionsAdapter = new ODataQueryOptionsAdapter<CustomerContact>(options);
        var request = new GetPagedCustomerContactRequest
        {
            QueryOptions = queryOptionsAdapter,
            SearchValue = searchValue,
            IsDeleted = isDeleted
        };
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<GetPagedCustomerContactResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetPagedCustomerContactAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Read")]
    [HttpGet("GetCustomerContact")]
    public async Task<ActionResult<ApiSuccessResult<GetCustomerContactResult>>> GetCustomerContactAsync(
        [FromQuery] string id,
        CancellationToken cancellationToken)
    {
        var request = new GetCustomerContactRequest { Id = id };
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<GetCustomerContactResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetCustomerContactAsync)}",
            Content = response
        });
    }

}


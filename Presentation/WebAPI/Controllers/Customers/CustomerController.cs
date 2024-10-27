// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Features.Customers.Commands;
using Application.Features.Customers.Queries;

using Domain.Entities;

using Infrastructure.DataAccessManagers.EFCores.ODatas;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

using WebAPI.Common.Filters;
using WebAPI.Common.Models;

namespace WebAPI.Controllers.Customers;

[Route("api/[controller]")]
public class CustomerController : BaseApiController
{
    public CustomerController(ISender sender) : base(sender)
    {
    }

    [ClaimBasedAuthorization("Create")]
    [HttpPost("CreateCustomer")]
    public async Task<ActionResult<ApiSuccessResult<CreateCustomerResult>>> CreateCustomerAsync(CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<CreateCustomerResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(CreateCustomerAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Update")]
    [HttpPost("UpdateCustomer")]
    public async Task<ActionResult<ApiSuccessResult<UpdateCustomerResult>>> UpdateCustomerAsync(UpdateCustomerRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<UpdateCustomerResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(UpdateCustomerAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Delete")]
    [HttpDelete("DeleteCustomer")]
    public async Task<ActionResult<ApiSuccessResult<DeleteCustomerResult>>> DeleteCustomerAsync(DeleteCustomerRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<DeleteCustomerResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(DeleteCustomerAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Read")]
    [HttpGet("GetPagedCustomer")]
    public async Task<ActionResult<ApiSuccessResult<GetPagedCustomerResult>>> GetPagedCustomerAsync(
        ODataQueryOptions<Customer> options,
        CancellationToken cancellationToken,
        [FromQuery] string searchValue,
        [FromQuery] bool isDeleted = false)
    {
        var queryOptionsAdapter = new ODataQueryOptionsAdapter<Customer>(options);
        var request = new GetPagedCustomerRequest
        {
            QueryOptions = queryOptionsAdapter,
            SearchValue = searchValue,
            IsDeleted = isDeleted
        };
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<GetPagedCustomerResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetPagedCustomerAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Read")]
    [HttpGet("GetCustomer")]
    public async Task<ActionResult<ApiSuccessResult<GetCustomerResult>>> GetCustomerAsync([FromQuery] string id, CancellationToken cancellationToken)
    {
        var request = new GetCustomerRequest { Id = id };
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<GetCustomerResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetCustomerAsync)}",
            Content = response
        });
    }


    [Authorize]
    [HttpGet("GetCustomerLookup")]
    public async Task<ActionResult<ApiSuccessResult<GetCustomerLookupResult>>> GetCustomerLookupAsync(
        CancellationToken cancellationToken)
    {
        var request = new GetCustomerLookupRequest();
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<GetCustomerLookupResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetCustomerLookupAsync)}",
            Content = response
        });
    }


}


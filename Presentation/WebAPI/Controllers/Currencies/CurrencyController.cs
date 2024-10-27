// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Features.Currencies.Commands;
using Application.Features.Currencies.Queries;

using Domain.Entities;

using Infrastructure.DataAccessManagers.EFCores.ODatas;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

using WebAPI.Common.Filters;
using WebAPI.Common.Models;

namespace WebAPI.Controllers.Currencies;

[Route("api/[controller]")]
public class CurrencyController : BaseApiController
{
    public CurrencyController(ISender sender) : base(sender)
    {
    }

    [ClaimBasedAuthorization("Create")]
    [HttpPost("CreateCurrency")]
    public async Task<ActionResult<ApiSuccessResult<CreateCurrencyResult>>> CreateCurrencyAsync(CreateCurrencyRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<CreateCurrencyResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(CreateCurrencyAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Update")]
    [HttpPost("UpdateCurrency")]
    public async Task<ActionResult<ApiSuccessResult<UpdateCurrencyResult>>> UpdateCurrencyAsync(UpdateCurrencyRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<UpdateCurrencyResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(UpdateCurrencyAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Delete")]
    [HttpDelete("DeleteCurrency")]
    public async Task<ActionResult<ApiSuccessResult<DeleteCurrencyResult>>> DeleteCurrencyAsync(DeleteCurrencyRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<DeleteCurrencyResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(DeleteCurrencyAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Read")]
    [HttpGet("GetPagedCurrency")]
    public async Task<ActionResult<ApiSuccessResult<GetPagedCurrencyResult>>> GetPagedCurrencyAsync(
        ODataQueryOptions<Currency> options,
        CancellationToken cancellationToken,
        [FromQuery] bool isDeleted = false)
    {
        var queryOptionsAdapter = new ODataQueryOptionsAdapter<Currency>(options);
        var request = new GetPagedCurrencyRequest { QueryOptions = queryOptionsAdapter, IsDeleted = isDeleted };
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<GetPagedCurrencyResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetPagedCurrencyAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Read")]
    [HttpGet("GetCurrency")]
    public async Task<ActionResult<ApiSuccessResult<GetCurrencyResult>>> GetCurrencyAsync(
        [FromQuery] string id,
        CancellationToken cancellationToken)
    {
        var request = new GetCurrencyRequest { Id = id };
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<GetCurrencyResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetCurrencyAsync)}",
            Content = response
        });
    }

    [Authorize]
    [HttpGet("GetCurrencyLookup")]
    public async Task<ActionResult<ApiSuccessResult<GetCurrencyLookupResult>>> GetCurrencyLookupAsync(
        CancellationToken cancellationToken)
    {
        var request = new GetCurrencyLookupRequest();
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<GetCurrencyLookupResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetCurrencyLookupAsync)}",
            Content = response
        });
    }


}


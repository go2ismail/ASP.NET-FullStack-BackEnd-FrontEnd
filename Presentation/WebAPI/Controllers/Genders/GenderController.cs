// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Features.Genders.Commands;
using Application.Features.Genders.Queries;

using Domain.Entities;

using Infrastructure.DataAccessManagers.EFCores.ODatas;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

using WebAPI.Common.Filters;
using WebAPI.Common.Models;

namespace WebAPI.Controllers.Genders;

[Route("api/[controller]")]
public class GenderController(ISender sender) : BaseApiController(sender)
{
    [ClaimBasedAuthorization("Create")]
    [HttpPost("CreateGender")]
    public async Task<ActionResult<ApiSuccessResult<CreateGenderResult>>> CreateGenderAsync(CreateGenderRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<CreateGenderResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(CreateGenderAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Update")]
    [HttpPost("UpdateGender")]
    public async Task<ActionResult<ApiSuccessResult<UpdateGenderResult>>> UpdateGenderAsync(UpdateGenderRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<UpdateGenderResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(UpdateGenderAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Delete")]
    [HttpDelete("DeleteGender")]
    public async Task<ActionResult<ApiSuccessResult<DeleteGenderResult>>> DeleteGenderAsync(DeleteGenderRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<DeleteGenderResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(DeleteGenderAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Read")]
    [HttpGet("GetPagedGender")]
    public async Task<ActionResult<ApiSuccessResult<GetPagedGenderResult>>> GetPagedGenderAsync(
        ODataQueryOptions<Gender> options,
        CancellationToken cancellationToken,
        [FromQuery] bool isDeleted = false)
    {
        var queryOptionsAdapter = new ODataQueryOptionsAdapter<Gender>(options);
        var request = new GetPagedGenderRequest { QueryOptions = queryOptionsAdapter, IsDeleted = isDeleted };
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<GetPagedGenderResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetPagedGenderAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Read")]
    [HttpGet("GetGender")]
    public async Task<ActionResult<ApiSuccessResult<GetGenderResult>>> GetGenderAsync(
        [FromQuery] string id,
        CancellationToken cancellationToken)
    {
        var request = new GetGenderRequest { Id = id };
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<GetGenderResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetGenderAsync)}",
            Content = response
        });
    }



    [Authorize]
    [HttpGet("GetGenderLookup")]
    public async Task<ActionResult<ApiSuccessResult<GetGenderLookupResult>>> GetGenderLookupAsync(
        CancellationToken cancellationToken)
    {
        var request = new GetGenderLookupRequest();
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<GetGenderLookupResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetGenderLookupAsync)}",
            Content = response
        });
    }
}

// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Features.Dashboards.Queries;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using WebAPI.Common.Filters;
using WebAPI.Common.Models;

namespace WebAPI.Controllers.Dashboards;

[Route("api/[controller]")]
public class DashboardController : BaseApiController
{
    public DashboardController(ISender sender) : base(sender)
    {
    }


    [ClaimBasedAuthorization("Read")]
    [HttpGet("GetDashboardMain")]
    public async Task<ActionResult<ApiSuccessResult<GetDashboardMainResult>>> GetDashboardMainAsync(
        CancellationToken cancellationToken)
    {
        var request = new GetDashboardMainRequest();
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<GetDashboardMainResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetDashboardMainAsync)}",
            Content = response
        });
    }


}


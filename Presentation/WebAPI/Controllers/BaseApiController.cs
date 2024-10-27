// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public abstract class BaseApiController : ControllerBase
{
    protected readonly ISender _sender;

    protected BaseApiController(ISender sender)
    {
        _sender = sender;
    }
}

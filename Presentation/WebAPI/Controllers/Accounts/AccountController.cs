// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Features.Accounts.Commands;

using Domain.Constants;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

using WebAPI.Common.Exceptions;
using WebAPI.Common.Filters;
using WebAPI.Common.Models;

namespace WebAPI.Controllers.Accounts;


[Route("api/[controller]")]
public class AccountController(ISender sender, IConfiguration configuration) : BaseApiController(sender)
{
    private readonly IConfiguration _configuration = configuration;

    [ClaimBasedAuthorization("Create")]
    [HttpPost("CreateUser")]
    public async Task<ActionResult<ApiSuccessResult<CreateUserResult>>> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<CreateUserResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(CreateUserAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Delete")]
    [HttpDelete("DeleteUser")]
    public async Task<ActionResult<ApiSuccessResult<DeleteUserResult>>> DeleteUserAsync(DeleteUserRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<DeleteUserResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(DeleteUserAsync)}",
            Content = response
        });
    }

    [Authorize]
    [HttpGet("CheckLoginStatus")]
    public IActionResult CheckLoginStatusAsync()
    {
        if (User.Identity == null)
        {
            throw new ApiException(
                StatusCodes.Status401Unauthorized,
                "Unauthorized: Token not valid or expired. Please re-login"
                );
        }

        if (!User.Identity.IsAuthenticated)
        {
            throw new ApiException(
                StatusCodes.Status401Unauthorized,
                "Unauthorized: Token not valid or expired. Please re-login"
                );
        }

        return Ok(new ApiSuccessResult<string>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(CheckLoginStatusAsync)}",
            Content = $"UserId: {User.FindFirst(ClaimTypes.NameIdentifier)?.Value}"
        });
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<ActionResult<ApiSuccessResult<LoginUserResult>>> LoginAsync(LoginUserRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken) ?? throw new ApiException(
                StatusCodes.Status401Unauthorized,
                "Invalid or expired token"
                );
        var accessToken = response.AccessToken;
        var refreshToken = response.RefreshToken;

        if (accessToken == null || refreshToken == null)
        {
            throw new ApiException(
                StatusCodes.Status401Unauthorized,
                "Access token or refresh token is null"
                );
        }

        bool useHttpOnlyCookieForToken = false;

        if (_configuration["Jwt:UseHttpOnlyCookieForToken"] != null)
        {
            bool.TryParse(_configuration["Jwt:UseHttpOnlyCookieForToken"], out useHttpOnlyCookieForToken);
        }

        if (useHttpOnlyCookieForToken)
        {

            var accessTokenCookieName = _configuration["Jwt:accessTokenCookieName"];
            var refreshTokenCookieName = _configuration["Jwt:refreshTokenCookieName"];

            if (!double.TryParse(_configuration["Jwt:ExpireInMinute"], out double expireInMinute))
            {
                expireInMinute = 15.0;
            }

            if (accessTokenCookieName != null)
            {
                HttpContext.Response.Cookies.Delete(accessTokenCookieName);
                HttpContext.Response.Cookies.Append(accessTokenCookieName, accessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddMinutes(expireInMinute)
                });
            }

            if (refreshTokenCookieName != null)
            {
                HttpContext.Response.Cookies.Delete(refreshTokenCookieName);
                HttpContext.Response.Cookies.Append(refreshTokenCookieName, refreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(TokenConsts.ExpiryInDays)
                });

            }
        }

        return Ok(new ApiSuccessResult<LoginUserResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(LoginAsync)}",
            Content = response
        });
    }

    [AllowAnonymous]
    [HttpPost("RegisterUser")]
    public async Task<ActionResult<ApiSuccessResult<RegisterUserResult>>> RegisterUserAsync(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<RegisterUserResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(RegisterUserAsync)}",
            Content = response
        });
    }

    [AllowAnonymous]
    [HttpPost("Logout")]
    public async Task<ActionResult<ApiSuccessResult<LogoutUserResult>>> LogoutAsync(LogoutUserRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);


        bool useHttpOnlyCookieForToken = false;

        if (_configuration["Jwt:UseHttpOnlyCookieForToken"] != null)
        {
            bool.TryParse(_configuration["Jwt:UseHttpOnlyCookieForToken"], out useHttpOnlyCookieForToken);
        }

        if (useHttpOnlyCookieForToken)
        {
            var accessTokenCookieName = _configuration["Jwt:accessTokenCookieName"];
            var refreshTokenCookieName = _configuration["Jwt:refreshTokenCookieName"];

            if (accessTokenCookieName != null) Response.Cookies.Delete(accessTokenCookieName);
            if (refreshTokenCookieName != null) Response.Cookies.Delete(refreshTokenCookieName);
        }


        return Ok(new ApiSuccessResult<LogoutUserResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(LogoutAsync)}",
            Content = response
        });
    }

    [AllowAnonymous]
    [HttpPost("RefreshAccessToken")]
    public async Task<ActionResult<ApiSuccessResult<GenerateRefreshTokenResult>>> RefreshAccessTokenAsync(GenerateRefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var refreshToken = HttpContext.Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
        {
            refreshToken = request.RefreshToken;
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new ApiException(
                    StatusCodes.Status400BadRequest,
                    "Refresh token has expired, please re-login"
                );
            }

        }


        request.RefreshToken = refreshToken;

        var response = await _sender.Send(request, cancellationToken);

        if (response == null)
        {
            throw new ApiException(
                StatusCodes.Status401Unauthorized,
                "Refresh token has expired, please re-login"
                );
        }



        var newAccessToken = response.AccessToken;
        var newRefreshToken = response.RefreshToken;

        if (newAccessToken == null || newRefreshToken == null)
        {
            throw new ApiException(
                StatusCodes.Status401Unauthorized,
                "Refresh token has expired, please re-login"
                );
        }

        bool useHttpOnlyCookieForToken = false;

        if (_configuration["Jwt:UseHttpOnlyCookieForToken"] != null)
        {
            bool.TryParse(_configuration["Jwt:UseHttpOnlyCookieForToken"], out useHttpOnlyCookieForToken);
        }

        if (useHttpOnlyCookieForToken)
        {
            var accessTokenCookieName = _configuration["Jwt:accessTokenCookieName"];
            var refreshTokenCookieName = _configuration["Jwt:refreshTokenCookieName"];

            double expireInMinute;
            if (!double.TryParse(_configuration["Jwt:ExpireInMinute"], out expireInMinute))
            {
                expireInMinute = 15.0;
            }

            if (accessTokenCookieName != null)
            {
                // Set cookie HttpOnly for accessToken
                HttpContext.Response.Cookies.Delete(accessTokenCookieName);
                HttpContext.Response.Cookies.Append(accessTokenCookieName, newAccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddMinutes(expireInMinute)
                });
            }

            if (refreshTokenCookieName != null)
            {
                // Set cookie HttpOnly for refreshToken
                HttpContext.Response.Cookies.Delete(refreshTokenCookieName);
                HttpContext.Response.Cookies.Append(refreshTokenCookieName, newRefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(TokenConsts.ExpiryInDays)
                });

            }
        }

        return Ok(new ApiSuccessResult<GenerateRefreshTokenResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(RefreshAccessTokenAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Update")]
    [HttpPost("AddRolesToUser")]
    public async Task<ActionResult<ApiSuccessResult<AddRolesToUserResult>>> AddRolesToUserAsync(AddRolesToUserRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<AddRolesToUserResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(AddRolesToUserAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Update")]
    [HttpDelete("DeleteRolesFromUser")]
    public async Task<ActionResult<ApiSuccessResult<DeleteRolesFromUserResult>>> DeleteRolesFromUserAsync(DeleteRolesFromUserRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<DeleteRolesFromUserResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(DeleteRolesFromUserAsync)}",
            Content = response
        });
    }

    [AllowAnonymous]
    [HttpGet("ConfirmEmail")]
    public async Task<ActionResult<ApiSuccessResult<ConfirmEmailResult>>> ConfirmEmailAsync(
        [FromQuery] string email,
        [FromQuery] string code,
        CancellationToken cancellationToken
        )
    {
        var request = new ConfirmEmailRequest { Email = email, Code = code };
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<ConfirmEmailResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(ConfirmEmailAsync)}",
            Content = response
        });
    }

    [AllowAnonymous]
    [HttpPost("ForgotPassword")]
    public async Task<ActionResult<ApiSuccessResult<ForgotPasswordResult>>> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<ForgotPasswordResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(ForgotPasswordAsync)}",
            Content = response
        });
    }

    [AllowAnonymous]
    [HttpGet("ForgotPasswordConfirmation")]
    public async Task<ActionResult<ApiSuccessResult<ForgotPasswordConfirmationResult>>> ForgotPasswordConfirmationAsync(
        [FromQuery] string email,
        [FromQuery] string code,
        [FromQuery] string tempPassword,
        CancellationToken cancellationToken)
    {
        var request = new ForgotPasswordConfirmationRequest { Email = email, TempPassword = tempPassword, Code = code };
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<ForgotPasswordConfirmationResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(ForgotPasswordConfirmationAsync)}",
            Content = response
        });
    }



}

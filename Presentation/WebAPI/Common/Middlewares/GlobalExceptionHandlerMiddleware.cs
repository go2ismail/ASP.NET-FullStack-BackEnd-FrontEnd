// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;

using System.Security.Authentication;

namespace WebAPI.Common.Middlewares;

public class GlobalExceptionHandlerMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext httpContext, IExceptionHandler customExceptionHandler)
    {
        try
        {
            await _next(httpContext);

            if (httpContext.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                await customExceptionHandler.TryHandleAsync(httpContext, new UnauthorizedAccessException("Unauthorized - Token missing or invalid"), CancellationToken.None);
            }
            else if (httpContext.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                await customExceptionHandler.TryHandleAsync(httpContext, new Exception("Forbidden - Access denied"), CancellationToken.None);
            }
        }
        catch (SecurityTokenExpiredException ex)
        {
            await customExceptionHandler.TryHandleAsync(httpContext, new SecurityTokenExpiredException("Token expired", ex), CancellationToken.None);
        }
        catch (AuthenticationException ex)
        {
            await customExceptionHandler.TryHandleAsync(httpContext, new AuthenticationException("Authentication failed", ex), CancellationToken.None);
        }
        catch (InvalidOperationException ex)
        {
            await customExceptionHandler.TryHandleAsync(httpContext, new InvalidOperationException("Invalid operation", ex), CancellationToken.None);
        }
        catch (UnauthorizedAccessException ex)
        {
            await customExceptionHandler.TryHandleAsync(httpContext, new UnauthorizedAccessException("Unauthorized access", ex), CancellationToken.None);
        }
        catch (Exception ex)
        {
            await customExceptionHandler.TryHandleAsync(httpContext, ex, CancellationToken.None);
        }
    }


}


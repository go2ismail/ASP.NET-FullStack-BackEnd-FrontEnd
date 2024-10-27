// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Domain.Exceptions;

using Infrastructure.DataAccessManagers.EFCores.Exceptions;

using Microsoft.AspNetCore.Diagnostics;

using WebAPI.Common.Exceptions;
using WebAPI.Common.Models;

namespace WebAPI.Common.Handlers;

public class CustomExceptionHandler : IExceptionHandler
{
    private readonly Dictionary<Type, Func<HttpContext, Exception, Task>> _exceptionHandlers;

    public CustomExceptionHandler()
    {
        _exceptionHandlers = new()
            {
                { typeof(DomainException), HandleDomainException },
                { typeof(TokenRepositoryException), HandleTokenException },
                { typeof(ApplicationException), HandleApplicationException },
                { typeof(ApiException), HandleApiException },
                { typeof(Exception), HandleException },
            };
    }
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var exceptionType = exception.GetType();

        if (_exceptionHandlers.TryGetValue(exceptionType, out Func<HttpContext, Exception, Task>? value))
        {
            await value.Invoke(httpContext, exception);
            return true;
        }
        else
        {
            await HandleException(httpContext, exception);
            return true;
        }

    }

    private async Task HandleDomainException(HttpContext httpContext, Exception ex)
    {
        var exception = (DomainException)ex;

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(new ApiErrorResult
        {
            Code = StatusCodes.Status500InternalServerError,
            Message = $"DomainException: {ex.Message}",
            Error = new Error(ex.InnerException?.Message, ex.Source, ex.StackTrace, ex.GetType().Name)
        });
    }

    private async Task HandleTokenException(HttpContext httpContext, Exception ex)
    {
        var exception = (TokenRepositoryException)ex;

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(new ApiErrorResult
        {
            Code = StatusCodes.Status500InternalServerError,
            Message = $"TokenException: {ex.Message}",
            Error = new Error(ex.InnerException?.Message, ex.Source, ex.StackTrace, ex.GetType().Name)
        });
    }

    private async Task HandleApplicationException(HttpContext httpContext, Exception ex)
    {
        var exception = (ApplicationException)ex;

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(new ApiErrorResult
        {
            Code = StatusCodes.Status500InternalServerError,
            Message = $"ApplicationException: {ex.Message}",
            Error = new Error(ex.InnerException?.Message, ex.Source, ex.StackTrace, ex.GetType().Name)
        });
    }

    private async Task HandleApiException(HttpContext httpContext, Exception ex)
    {
        var exception = (ApiException)ex;

        httpContext.Response.StatusCode = exception.Code;

        await httpContext.Response.WriteAsJsonAsync(new ApiErrorResult
        {
            Code = exception.Code,
            Message = $"ApiException: {exception.Message}",
            Error = new Error(ex.InnerException?.Message, exception.Source, exception.StackTrace, exception.GetType().Name)
        });
    }

    private async Task HandleException(HttpContext httpContext, Exception ex)
    {
        var exception = ex;

        await httpContext.Response.WriteAsJsonAsync(new ApiErrorResult
        {
            Code = httpContext.Response.StatusCode != 200 ? httpContext.Response.StatusCode : StatusCodes.Status500InternalServerError,
            Message = $"Exception: {ex.Message}",
            Error = new Error(ex.InnerException?.Message, ex.Source, ex.StackTrace, ex.GetType().Name)
        });
    }
}

// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

namespace WebAPI.Common.Models;

public class Error(
    string? innerException,
    string? source,
    string? stackTrace,
    string? exceptionType)
{
    public string Ref { get; set; } = "https://datatracker.ietf.org/doc/html/rfc9110";
    public string? ExceptionType { get; init; } = exceptionType;
    public string? InnerException { get; init; } = innerException;
    public string? Source { get; init; } = source?.Trim();
    public string? StackTrace { get; init; } = stackTrace?.Trim();
}

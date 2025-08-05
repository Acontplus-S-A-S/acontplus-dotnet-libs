namespace Acontplus.Persistence.Common.Exceptions;

public record SqlErrorInfo(
    ErrorType ErrorType,
    string Code,
    string Message,
    Exception? Exception = null)
{
    public string? StackTrace => Exception?.StackTrace;
}

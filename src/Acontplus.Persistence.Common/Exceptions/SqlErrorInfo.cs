namespace Acontplus.Persistence.Common.Exceptions;

public record SqlErrorInfo(
    ErrorType Type,
    string Code,
    string Message,
    Exception? Exception = null)
{
    public string? StackTrace => Exception?.StackTrace;
}
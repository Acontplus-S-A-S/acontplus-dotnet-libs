using Acontplus.Core.Domain.Enums;

namespace Acontplus.Persistence.Shared.Exceptions;

public record SqlErrorInfo(
    ErrorType Type,
    string Code,
    string Message,
    Exception? Exception = null)
{
    public string? StackTrace => Exception?.StackTrace;
}
namespace Acontplus.Core.Domain.Exceptions;

public abstract class DomainException(ErrorType type, string code, string message, Exception? inner = null)
    : Exception(message, inner)
{
    public ErrorType ErrorType { get; } = type;
    public string ErrorCode { get; } = code;
}
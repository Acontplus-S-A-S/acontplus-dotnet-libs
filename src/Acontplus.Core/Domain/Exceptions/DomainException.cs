namespace Acontplus.Core.Domain.Exceptions;

public abstract class DomainException : Exception
{
    public ErrorType ErrorType { get; }
    public string ErrorCode { get; }
    public DomainException(ErrorType type, string code, string message, Exception? inner = null)
        : base(message, inner)
    {
        ErrorType = type;
        ErrorCode = code;
    }
}
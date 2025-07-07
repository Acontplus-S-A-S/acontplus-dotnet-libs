namespace Acontplus.Core.Domain.Exceptions;

public class GenericDomainException : DomainException
{
    public GenericDomainException(ErrorType type, string code, string message, Exception? inner = null)
        : base(type, code, message, inner)
    {
    }
}
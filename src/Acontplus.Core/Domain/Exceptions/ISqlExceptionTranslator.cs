namespace Acontplus.Core.Domain.Exceptions;

public interface ISqlExceptionTranslator
{
    bool IsTransient(Exception ex);
    DomainException Translate(Exception ex);
}
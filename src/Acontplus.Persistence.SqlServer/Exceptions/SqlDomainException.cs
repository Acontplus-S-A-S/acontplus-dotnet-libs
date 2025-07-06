using Acontplus.Core.Domain.Enums;

namespace Acontplus.Persistence.SqlServer.Exceptions;

public class SqlDomainException(ErrorType errorType, string code, string message, SqlException originalException)
    : Exception(message)
{
    public ErrorType ErrorType { get; } = errorType;
    public string Code { get; } = code;
    public SqlException OriginalException { get; } = originalException;
}
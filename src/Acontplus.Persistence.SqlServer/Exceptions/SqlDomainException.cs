namespace Acontplus.Persistence.SqlServer.Exceptions;

public class SqlDomainException(SqlErrorType errorType, string message, SqlException originalException)
    : Exception(message)
{
    public SqlErrorType ErrorType { get; } = errorType;
    public SqlException OriginalException { get; } = originalException;
}
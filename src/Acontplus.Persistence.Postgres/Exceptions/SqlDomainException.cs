namespace Acontplus.Persistence.Postgres.Exceptions;

public class SqlDomainException : DomainException
{
    public SqlDomainException(SqlErrorInfo errorInfo)
        : base(errorInfo.Type, errorInfo.Code, errorInfo.Message, errorInfo.Exception)
    {
    }
}
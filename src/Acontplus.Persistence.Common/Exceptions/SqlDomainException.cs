namespace Acontplus.Persistence.Common.Exceptions;

public class SqlDomainException : DomainException
{
    public SqlDomainException(SqlErrorInfo sqlErrorInfo)
        : base(sqlErrorInfo.ErrorType, sqlErrorInfo.Code, sqlErrorInfo.Message, sqlErrorInfo.Exception)
    {
    }
}

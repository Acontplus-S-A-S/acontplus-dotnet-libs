using Acontplus.Core.Domain.Enums;

namespace Acontplus.Persistence.SqlServer.Exceptions;

public class SqlDomainException(SqlErrorInfo errorInfo) : Exception(errorInfo.Message, errorInfo.Exception)
{
    public ErrorType ErrorType { get; } = errorInfo.Type;
    public string ErrorCode { get; } = errorInfo.Code;
}
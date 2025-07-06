namespace Acontplus.Persistence.SqlServer.Exceptions;

public record SqlErrorInfo(SqlErrorType Type, string Message, SqlException OriginalException);
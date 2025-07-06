using Acontplus.Core.Domain.Enums;

namespace Acontplus.Persistence.SqlServer.Exceptions;

public record SqlErrorInfo(ErrorType Type, string Code, string Message, SqlException OriginalException);

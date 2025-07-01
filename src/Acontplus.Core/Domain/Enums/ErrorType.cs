namespace Acontplus.Core.Domain.Enums;

public enum ErrorType
{
    Validation,
    NotFound,
    Conflict,
    Unauthorized,
    Forbidden,
    Internal,
    External,
    RateLimited,
    ServiceUnavailable,
    Timeout
}
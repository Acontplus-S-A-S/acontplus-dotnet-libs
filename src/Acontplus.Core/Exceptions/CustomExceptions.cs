using System.Net;

namespace Acontplus.Core.Exceptions;

public abstract class ApiException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string ErrorCode { get; }

    protected ApiException(
        HttpStatusCode statusCode,
        string errorCode,
        string message) : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }
}

public class NotFoundException : ApiException
{
    public NotFoundException(string resourceName, object key)
        : base(HttpStatusCode.NotFound,
              "NOT_FOUND",
              $"Resource '{resourceName}' with key '{key}' was not found")
    {
    }
}

public class ConflictException : ApiException
{
    public ConflictException(string resourceName, string conflictDetail)
        : base(HttpStatusCode.Conflict,
              "CONFLICT",
              $"Conflict occurred with resource '{resourceName}': {conflictDetail}")
    {
    }
}

public class ValidationException : ApiException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(IDictionary<string, string[]> errors)
        : base(HttpStatusCode.BadRequest,
              "VALIDATION_FAILED",
              "One or more validation errors occurred")
    {
        Errors = errors;
    }
}

public class UnauthorizedException : ApiException
{
    public UnauthorizedException(string message)
        : base(HttpStatusCode.Unauthorized,
              "UNAUTHORIZED",
              message)
    {
    }
}
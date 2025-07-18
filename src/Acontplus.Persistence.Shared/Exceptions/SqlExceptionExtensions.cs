public static class SqlExceptionExtensions
{
    private static readonly
        ImmutableDictionary<ErrorType, Func<string, string, string?, Dictionary<string, object>?, DomainError>>
        ErrorMappers =
            new Dictionary<ErrorType, Func<string, string, string?, Dictionary<string, object>?, DomainError>>
            {
                // Client Errors (4xx)
                [ErrorType.Validation] = DomainError.Validation,
                [ErrorType.BadRequest] = DomainError.BadRequest,
                [ErrorType.NotFound] = DomainError.NotFound,
                [ErrorType.Conflict] = DomainError.Conflict,
                [ErrorType.Unauthorized] = DomainError.Unauthorized,
                [ErrorType.Forbidden] = DomainError.Forbidden,
                [ErrorType.MethodNotAllowed] = (code, msg, target, details) =>
                    new DomainError(ErrorType.MethodNotAllowed, code, msg, target, details),
                [ErrorType.NotAcceptable] = (code, msg, target, details) =>
                    new DomainError(ErrorType.NotAcceptable, code, msg, target, details),
                [ErrorType.PayloadTooLarge] = (code, msg, target, details) =>
                    new DomainError(ErrorType.PayloadTooLarge, code, msg, target, details),
                [ErrorType.RateLimited] = DomainError.RateLimited,

                // Server Errors (5xx)
                [ErrorType.Internal] = DomainError.Internal,
                [ErrorType.External] = DomainError.External,
                [ErrorType.ServiceUnavailable] = DomainError.ServiceUnavailable,
                [ErrorType.Timeout] = DomainError.Timeout,
                [ErrorType.RequestTimeout] = (code, msg, target, details) =>
                    new DomainError(ErrorType.RequestTimeout, code, msg, target, details),
            }.ToImmutableDictionary();

    public static DomainError ToDomainError(
        this SqlDomainException ex,
        string? target = null,
        Dictionary<string, object>? details = null)
    {
        if (ErrorMappers.TryGetValue(ex.ErrorType, out var mapper))
        {
            return mapper(ex.ErrorCode, ex.Message, target, details);
        }

        // Default mapping for unmapped error types
        return ex.ErrorType switch
        {
            // Additional client error mappings
            ErrorType.UriTooLong => new DomainError(ErrorType.UriTooLong, ex.ErrorCode, ex.Message, target, details),
            ErrorType.UnsupportedMediaType => new DomainError(ErrorType.UnsupportedMediaType, ex.ErrorCode, ex.Message,
                target, details),
            ErrorType.PreconditionFailed => new DomainError(ErrorType.PreconditionFailed, ex.ErrorCode, ex.Message,
                target, details),

            // Additional server error mappings
            ErrorType.NotImplemented => new DomainError(ErrorType.NotImplemented, ex.ErrorCode, ex.Message, target,
                details),
            ErrorType.HttpVersionNotSupported => new DomainError(ErrorType.HttpVersionNotSupported, ex.ErrorCode,
                ex.Message, target, details),

            _ => DomainError.Internal(
                $"SQL_{ex.ErrorCode}",
                $"Unmapped SQL error: {ex.Message}",
                target,
                details?.Union(new Dictionary<string, object>
                {
                    ["originalErrorType"] = ex.ErrorType.ToString(),
                    ["sqlErrorCode"] = ex.ErrorCode
                }).ToDictionary(kvp => kvp.Key, kvp => kvp.Value))
        };
    }

    /// <summary>
    /// Converts multiple SqlDomainExceptions to a DomainErrors collection
    /// </summary>
    /// <param name="exceptions">Collection of SQL domain exceptions</param>
    /// <param name="target">Optional target of the errors</param>
    /// <param name="commonDetails">Optional common details for all errors</param>
    /// <returns>DomainErrors containing all converted exceptions</returns>
    public static DomainErrors ToDomainErrors(
        this IEnumerable<SqlDomainException> exceptions,
        string? target = null,
        Dictionary<string, object>? commonDetails = null)
    {
        var domainErrors = exceptions
            .Select(ex => ex.ToDomainError(target, commonDetails))
            .ToList();

        return domainErrors.Count switch
        {
            0 => throw new ArgumentException("No exceptions provided", nameof(exceptions)),
            1 => DomainErrors.Single(domainErrors[0]),
            _ => DomainErrors.Multiple(domainErrors)
        };
    }
}
namespace Acontplus.Core.Domain.Enums;

public enum ErrorType
{
    // Client Errors (4xx)
    Validation,              // 422 - Unprocessable Entity
    BadRequest,              // 400 - Bad Request (syntax errors)
    NotFound,                // 404 - Not Found
    Conflict,                // 409 - Conflict
    Unauthorized,            // 401 - Unauthorized
    Forbidden,               // 403 - Forbidden
    MethodNotAllowed,        // 405 - Method Not Allowed
    NotAcceptable,           // 406 - Not Acceptable
    PayloadTooLarge,         // 413 - Payload Too Large
    UriTooLong,              // 414 - URI Too Long
    UnsupportedMediaType,    // 415 - Unsupported Media Type
    RangeNotSatisfiable,     // 416 - Range Not Satisfiable
    ExpectationFailed,       // 417 - Expectation Failed
    PreconditionFailed,      // 412 - Precondition Failed
    PreconditionRequired,    // 428 - Precondition Required
    RequestHeadersTooLarge,  // 431 - Request Header Fields Too Large
    UnavailableForLegal,     // 451 - Unavailable For Legal Reasons
    RateLimited,             // 429 - Too Many Requests

    // Server Errors (5xx)
    Internal,                // 500 - Internal Server Error
    NotImplemented,          // 501 - Not Implemented
    External,                // 502 - Bad Gateway
    ServiceUnavailable,      // 503 - Service Unavailable
    Timeout,                 // 504 - Gateway Timeout (for external services)
    RequestTimeout,          // 408 - Request Timeout (for client timeouts)
    HttpVersionNotSupported, // 505 - HTTP Version Not Supported
    InsufficientStorage,     // 507 - Insufficient Storage
    LoopDetected,            // 508 - Loop Detected
    NotExtended,             // 510 - Not Extended
    NetworkAuthRequired      // 511 - Network Authentication Required
}
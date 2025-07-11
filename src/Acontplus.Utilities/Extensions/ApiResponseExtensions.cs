using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Acontplus.Core.DTOs.Responses;

namespace Acontplus.Utilities.Extensions;

public static class ApiResponseExtensions
{
    /// <summary>
    /// Converts generic ApiResponse<T> to base ApiResponse
    /// </summary>
    public static ApiResponse ToBaseResponse<T>(this ApiResponse<T> response)
    {
        var options = new ApiResponseOptions
        {
            Message = response.Message,
            Errors = response.Errors,
            Warnings = response.Warnings,
            Metadata = response.Metadata,
            CorrelationId = response.CorrelationId,
            StatusCode = response.StatusCode,
            TraceId = response.TraceId,
            Timestamp = response.Timestamp
        };

        if (response.IsSuccess)
        {
            return ApiResponse.Success(response.Data, options);
        }
        else // Assuming error/warning cases
        {
            return ApiResponse.Failure(response.Errors ?? Array.Empty<ApiError>(), options);
        }
    }
    /// <summary>
    /// Converts ApiResponse<T> to IActionResult
    /// </summary>
    public static IActionResult ToActionResult<T>(this ApiResponse<T> response)
    {
        return response.ToBaseResponse().ToActionResult();
    }

    /// <summary>
    /// Converts ApiResponse<T> to IResult (for Minimal APIs)
    /// </summary>
    public static IResult ToMinimalApiResult<T>(this ApiResponse<T> response)
    {
        return response.ToBaseResponse().ToMinimalApiResult();
    }

    /// <summary>
    /// Converts base ApiResponse to IActionResult with full status code support
    /// </summary>
    public static IActionResult ToActionResult(this ApiResponse response)
    {
        return response.StatusCode switch
        {
            // Success (2xx)
            HttpStatusCode.OK => new OkObjectResult(response),
            HttpStatusCode.Created => new ObjectResult(response) { StatusCode = (int)HttpStatusCode.Created },
            HttpStatusCode.Accepted => new ObjectResult(response) { StatusCode = (int)HttpStatusCode.Accepted },
            HttpStatusCode.NoContent => new NoContentResult(),

            // Client Errors (4xx)
            HttpStatusCode.BadRequest => new BadRequestObjectResult(response),
            HttpStatusCode.Unauthorized => new UnauthorizedObjectResult(response),
            HttpStatusCode.Forbidden => new ObjectResult(response) { StatusCode = (int)HttpStatusCode.Forbidden },
            HttpStatusCode.NotFound => new NotFoundObjectResult(response),
            HttpStatusCode.Conflict => new ConflictObjectResult(response),
            HttpStatusCode.UnprocessableEntity => new ObjectResult(response)
            { StatusCode = (int)HttpStatusCode.UnprocessableEntity },
            HttpStatusCode.TooManyRequests => new ObjectResult(response)
            { StatusCode = (int)HttpStatusCode.TooManyRequests },
            HttpStatusCode.RequestEntityTooLarge => new ObjectResult(response)
            { StatusCode = (int)HttpStatusCode.RequestEntityTooLarge },
            HttpStatusCode.RequestUriTooLong => new ObjectResult(response)
            { StatusCode = (int)HttpStatusCode.RequestUriTooLong },
            HttpStatusCode.UnsupportedMediaType => new ObjectResult(response)
            { StatusCode = (int)HttpStatusCode.UnsupportedMediaType },
            (HttpStatusCode)428 => new ObjectResult(response) { StatusCode = 428 }, // PreconditionRequired
            (HttpStatusCode)431 => new ObjectResult(response) { StatusCode = 431 }, // RequestHeaderFieldsTooLarge
            (HttpStatusCode)451 => new ObjectResult(response) { StatusCode = 451 }, // UnavailableForLegalReasons

            // Server Errors (5xx)
            HttpStatusCode.InternalServerError => new ObjectResult(response)
            { StatusCode = (int)HttpStatusCode.InternalServerError },
            HttpStatusCode.NotImplemented => new ObjectResult(response)
            { StatusCode = (int)HttpStatusCode.NotImplemented },
            HttpStatusCode.BadGateway => new ObjectResult(response)
            { StatusCode = (int)HttpStatusCode.BadGateway },
            HttpStatusCode.ServiceUnavailable => new ObjectResult(response)
            { StatusCode = (int)HttpStatusCode.ServiceUnavailable },
            HttpStatusCode.GatewayTimeout => new ObjectResult(response)
            { StatusCode = (int)HttpStatusCode.GatewayTimeout },
            (HttpStatusCode)507 => new ObjectResult(response) { StatusCode = 507 }, // InsufficientStorage
            (HttpStatusCode)508 => new ObjectResult(response) { StatusCode = 508 }, // LoopDetected
            (HttpStatusCode)510 => new ObjectResult(response) { StatusCode = 510 }, // NotExtended
            (HttpStatusCode)511 => new ObjectResult(response) { StatusCode = 511 }, // NetworkAuthenticationRequired

            _ => new ObjectResult(response) { StatusCode = (int)HttpStatusCode.InternalServerError }
        };
    }

    /// <summary>
    /// Converts ApiResponse to IResult for Minimal APIs
    /// </summary>
    public static IResult ToMinimalApiResult(this ApiResponse response)
    {
        return response.StatusCode switch
        {
            // Success (2xx)
            HttpStatusCode.OK => TypedResults.Ok(response),
            HttpStatusCode.Created => TypedResults.Created(string.Empty, response),
            HttpStatusCode.Accepted => TypedResults.Accepted(string.Empty, response),
            HttpStatusCode.NoContent => TypedResults.NoContent(),

            // Client Errors (4xx)
            HttpStatusCode.BadRequest => TypedResults.BadRequest(response),
            HttpStatusCode.Unauthorized => TypedResults.Unauthorized(),
            HttpStatusCode.Forbidden => TypedResults.Forbid(),
            HttpStatusCode.NotFound => TypedResults.NotFound(response),
            HttpStatusCode.Conflict => TypedResults.Conflict(response),
            HttpStatusCode.UnprocessableEntity => TypedResults.UnprocessableEntity(response),
            HttpStatusCode.TooManyRequests => TypedResults.StatusCode((int)HttpStatusCode.TooManyRequests),
            HttpStatusCode.RequestEntityTooLarge => TypedResults.StatusCode((int)HttpStatusCode.RequestEntityTooLarge),
            HttpStatusCode.RequestUriTooLong => TypedResults.StatusCode((int)HttpStatusCode.RequestUriTooLong),
            HttpStatusCode.UnsupportedMediaType => TypedResults.StatusCode((int)HttpStatusCode.UnsupportedMediaType),
            (HttpStatusCode)428 => TypedResults.StatusCode(428), // PreconditionRequired
            (HttpStatusCode)431 => TypedResults.StatusCode(431), // RequestHeaderFieldsTooLarge
            (HttpStatusCode)451 => TypedResults.StatusCode(451), // UnavailableForLegalReasons

            // Server Errors (5xx)
            HttpStatusCode.InternalServerError => TypedResults.StatusCode((int)HttpStatusCode.InternalServerError),
            HttpStatusCode.NotImplemented => TypedResults.StatusCode((int)HttpStatusCode.NotImplemented),
            HttpStatusCode.BadGateway => TypedResults.StatusCode((int)HttpStatusCode.BadGateway),
            HttpStatusCode.ServiceUnavailable => TypedResults.StatusCode((int)HttpStatusCode.ServiceUnavailable),
            HttpStatusCode.GatewayTimeout => TypedResults.StatusCode((int)HttpStatusCode.GatewayTimeout),
            (HttpStatusCode)507 => TypedResults.StatusCode(507), // InsufficientStorage
            (HttpStatusCode)508 => TypedResults.StatusCode(508), // LoopDetected
            (HttpStatusCode)510 => TypedResults.StatusCode(510), // NotExtended
            (HttpStatusCode)511 => TypedResults.StatusCode(511), // NetworkAuthenticationRequired

            _ => TypedResults.StatusCode((int)HttpStatusCode.InternalServerError)
        };
    }

    /// <summary>
    /// Adds pagination metadata with enhanced details
    /// </summary>
    public static Dictionary<string, object> WithPagination(
        this Dictionary<string, object> metadata,
        int page,
        int pageSize,
        long totalItems,
        Func<int, string>? pageUrlGenerator = null)
    {
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var pagination = new Dictionary<string, object>
        {
            [ApiMetadataKeys.Page] = page,
            [ApiMetadataKeys.PageSize] = pageSize,
            [ApiMetadataKeys.TotalItems] = totalItems,
            [ApiMetadataKeys.TotalPages] = totalPages,
            [ApiMetadataKeys.HasNext] = page < totalPages,
            [ApiMetadataKeys.HasPrev] = page > 1
        };

        if (pageUrlGenerator != null)
        {
            pagination[ApiMetadataKeys.Links] = new
            {
                first = pageUrlGenerator(1),
                prev = page > 1 ? pageUrlGenerator(page - 1) : null,
                next = page < totalPages ? pageUrlGenerator(page + 1) : null,
                last = pageUrlGenerator(totalPages)
            };
        }

        metadata[ApiMetadataKeys.Pagination] = pagination;
        return metadata;
    }

    /// <summary>
    /// Adds execution time metadata with precision control
    /// </summary>
    public static Dictionary<string, object> WithExecutionTime(
        this Dictionary<string, object>? metadata,
        TimeSpan executionTime,
        int precision = 2)
    {
        var result = metadata ?? new Dictionary<string, object>();
        result[ApiMetadataKeys.Duration] = new
        {
            Milliseconds = Math.Round(executionTime.TotalMilliseconds, precision),
            Seconds = Math.Round(executionTime.TotalSeconds, precision),
            Formatted = $"{Math.Round(executionTime.TotalMilliseconds, precision)}ms"
        };
        return result;
    }

    /// <summary>
    /// Adds correlation ID to metadata if not already present
    /// </summary>
    public static Dictionary<string, object> WithCorrelationId(
        this Dictionary<string, object>? metadata,
        string correlationId)
    {
        var result = metadata ?? new Dictionary<string, object>();
        if (!result.ContainsKey(ApiMetadataKeys.CorrelationId))
        {
            result[ApiMetadataKeys.CorrelationId] = correlationId;
        }
        return result;
    }
}
using Acontplus.Core.Domain.Common;
using Acontplus.Core.Domain.Enums;
using Acontplus.Core.DTOs.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Acontplus.Utilities.Extensions;


public static class ResultExtensions
{
    // Single error handling
    public static IActionResult ToActionResult<TValue>(
        this Result<TValue, DomainError> result,
        string? correlationId = null)
    {
        return result.Match(
            onSuccess: value => CreateSuccessResponse(value, correlationId),
            onFailure: error => CreateErrorResponse(error, correlationId)
        );
    }

    // Multiple errors handling
    public static IActionResult ToActionResult<TValue>(
        this Result<TValue, DomainErrors> result,
        string? correlationId = null)
    {
        return result.Match(
            onSuccess: value => CreateSuccessResponse(value, correlationId),
            onFailure: errors => CreateErrorResponse(errors, correlationId)
        );
    }

    // Success with warnings handling
    public static IActionResult ToActionResult<TValue>(
        this Result<SuccessWithWarnings<TValue>, DomainError> result,
        string? correlationId = null)
    {
        return result.Match(
            onSuccess: successWithWarnings =>
            {
                if (successWithWarnings.HasWarnings)
                {
                    // We know Warnings is not null here because HasWarnings checked it
                    return CreateWarningResponse(
                        successWithWarnings.Value,
                        successWithWarnings.Warnings!.Value, // Use .Value to get non-null DomainWarnings
                        correlationId);
                }
                return CreateSuccessResponse(successWithWarnings.Value, correlationId);
            },
            onFailure: error => CreateErrorResponse(error, correlationId)
        );
    }

    // Async variants
    public static async Task<IActionResult> ToActionResultAsync<TValue>(
        this Task<Result<TValue, DomainError>> resultTask,
        string? correlationId = null)
    {
        var result = await resultTask;
        return result.ToActionResult(correlationId);
    }

    public static async Task<IActionResult> ToActionResultAsync<TValue>(
        this Task<Result<TValue, DomainErrors>> resultTask,
        string? correlationId = null)
    {
        var result = await resultTask;
        return result.ToActionResult(correlationId);
    }

    public static async Task<IActionResult> ToActionResultAsync<TValue>(
        this Task<Result<SuccessWithWarnings<TValue>, DomainError>> resultTask,
        string? correlationId = null)
    {
        var result = await resultTask;
        return result.ToActionResult(correlationId);
    }

    public static IResult ToMinimalApiResult<TValue>(
        this Result<TValue, DomainError> result,
        string? correlationId = null)
    {
        return result.Match(
            onSuccess: value => TypedResults.Ok(ApiResponse<TValue>.Success(value, correlationId: correlationId)),
            onFailure: error => CreateErrorResult(error, correlationId)
        );
    }

    public static IResult ToMinimalApiResult<TValue>(
        this Result<TValue, DomainErrors> result,
        string? correlationId = null)
    {
        return result.Match(
            onSuccess: value => TypedResults.Ok(ApiResponse<TValue>.Success(value, correlationId: correlationId)),
            onFailure: errors => CreateErrorResult(errors, correlationId)
        );
    }
    // Helper methods for creating responses

    private static IResult CreateErrorResult(DomainError error, string? correlationId)
    {
        var statusCode = error.Type.ToHttpStatusCode();
        var response = ApiResponse.Failure(
            error.ToApiError(),
            statusCode: statusCode,
            correlationId: correlationId);

        return Results.Json(response, statusCode: (int)statusCode);
    }
    private static IResult CreateErrorResult(DomainErrors errors, string? correlationId)
    {
        var primaryErrorType = errors.GetMostSevereErrorType();
        var statusCode = primaryErrorType.ToHttpStatusCode();

        var response = ApiResponse.Failure(
            errors.ToApiErrors(),
            statusCode: statusCode,
            correlationId: correlationId,
            message: errors.GetAggregateErrorMessage());

        return Results.Json(response, statusCode: (int)statusCode);
    }
    private static IActionResult CreateSuccessResponse<TValue>(TValue value, string? correlationId)
    {
        var response = ApiResponse<TValue>.Success(
            data: value,
            correlationId: correlationId,
            statusCode: HttpStatusCode.OK
        );
        return new OkObjectResult(response);
    }

    private static IActionResult CreateWarningResponse<TValue>(
        TValue value,
        DomainWarnings? warnings, // Keep as nullable
        string? correlationId)
    {
        var response = ApiResponse<TValue>.Warning(
            data: value,
            warnings: warnings?.ToApiErrors() ?? Enumerable.Empty<ApiError>(),
            correlationId: correlationId,
            statusCode: HttpStatusCode.OK
        );
        return new OkObjectResult(response);
    }
    private static IActionResult CreateErrorResponse(DomainError error, string? correlationId)
    {
        var statusCode = GetHttpStatusCode(error.Type);
        var response = ApiResponse.Failure(
            error: error.ToApiError(),
            correlationId: correlationId,
            statusCode: statusCode
        );

        return CreateActionResult(response, statusCode);
    }

    private static IActionResult CreateErrorResponse(DomainErrors errors, string? correlationId)
    {
        var primaryErrorType = GetMostSevereErrorType(errors.Errors);
        var statusCode = GetHttpStatusCode(primaryErrorType);

        var response = ApiResponse.Failure(
            errors: errors.ToApiErrors(),
            correlationId: correlationId,
            statusCode: statusCode
        );

        return CreateActionResult(response, statusCode);
    }

    private static HttpStatusCode GetHttpStatusCode(ErrorType errorType) => errorType switch
    {
        ErrorType.NotFound => HttpStatusCode.NotFound,
        ErrorType.Validation => HttpStatusCode.BadRequest,
        ErrorType.Conflict => HttpStatusCode.Conflict,
        ErrorType.Unauthorized => HttpStatusCode.Unauthorized,
        ErrorType.Forbidden => HttpStatusCode.Forbidden,
        ErrorType.RateLimited => HttpStatusCode.TooManyRequests,
        ErrorType.External => HttpStatusCode.BadGateway,
        ErrorType.Internal => HttpStatusCode.InternalServerError,
        _ => HttpStatusCode.InternalServerError
    };

    private static IActionResult CreateActionResult(ApiResponse response, HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.OK => new OkObjectResult(response),
            HttpStatusCode.BadRequest => new BadRequestObjectResult(response),
            HttpStatusCode.Unauthorized => new UnauthorizedObjectResult(response),
            HttpStatusCode.Forbidden => new ObjectResult(response) { StatusCode = (int)HttpStatusCode.Forbidden },
            HttpStatusCode.NotFound => new NotFoundObjectResult(response),
            HttpStatusCode.Conflict => new ConflictObjectResult(response),
            HttpStatusCode.TooManyRequests => new ObjectResult(response) { StatusCode = (int)HttpStatusCode.TooManyRequests },
            HttpStatusCode.BadGateway => new ObjectResult(response) { StatusCode = (int)HttpStatusCode.BadGateway },
            _ => new ObjectResult(response) { StatusCode = (int)HttpStatusCode.InternalServerError }
        };
    }

    private static ErrorType GetMostSevereErrorType(IReadOnlyList<DomainError> errors)
    {
        var severity = new Dictionary<ErrorType, int>
        {
            [ErrorType.Validation] = 1,
            [ErrorType.NotFound] = 2,
            [ErrorType.Conflict] = 3,
            [ErrorType.Unauthorized] = 4,
            [ErrorType.Forbidden] = 5,
            [ErrorType.RateLimited] = 6,
            [ErrorType.External] = 7,
            [ErrorType.Internal] = 8
        };

        return errors.Select(e => e.Type).OrderByDescending(t => severity.GetValueOrDefault(t, 0)).First();
    }
}
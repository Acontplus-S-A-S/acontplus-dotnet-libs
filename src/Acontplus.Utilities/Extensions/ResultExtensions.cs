using Acontplus.Core.Domain.Common;
using Acontplus.Core.Domain.Enums;
using Acontplus.Core.DTOs.Responses;
using Acontplus.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Acontplus.Utilities.Extensions;

public static class ResultExtensions
{
    #region Action Results (Controller-style)

    // Single error handling
    public static IActionResult ToActionResult<TValue>(
        this Result<TValue, DomainError> result,
        string? correlationId = null)
    {
        return result.Match(
            onSuccess: value => CreateSuccessResponse(value, correlationId),
            onFailure: error => error.ToApiResponse<TValue>(correlationId).ToActionResult()
        );
    }

    // Multiple errors handling
    public static IActionResult ToActionResult<TValue>(
        this Result<TValue, DomainErrors> result,
        string? correlationId = null)
    {
        return result.Match(
            onSuccess: value => CreateSuccessResponse(value, correlationId),
            onFailure: errors => errors.ToApiResponse<TValue>(correlationId).ToActionResult()
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
                    return CreateWarningResponse(
                        successWithWarnings.Value,
                        successWithWarnings.Warnings!.Value,
                        correlationId);
                }

                return CreateSuccessResponse(successWithWarnings.Value, correlationId);
            },
            onFailure: error => error.ToApiResponse<TValue>(correlationId).ToActionResult()
        );
    }

    #endregion

    #region Async Action Results

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

    #endregion

    #region Minimal API Results (IResult)

    // Single error handling
    public static IResult ToMinimalApiResult<TValue>(
        this Result<TValue, DomainError> result,
        string? correlationId = null)
    {
        return result.Match(
            onSuccess: value => Results.Ok(ApiResponse<TValue>.Success(value, correlationId: correlationId)),
            onFailure: error => error.ToApiResponse<TValue>(correlationId).ToMinimalApiResult()
        );
    }

    // Multiple errors handling
    public static IResult ToMinimalApiResult<TValue>(
        this Result<TValue, DomainErrors> result,
        string? correlationId = null)
    {
        return result.Match(
            onSuccess: value => Results.Ok(ApiResponse<TValue>.Success(value, correlationId: correlationId)),
            onFailure: errors => errors.ToApiResponse<TValue>(correlationId).ToMinimalApiResult()
        );
    }

    // Success with warnings handling
    public static IResult ToMinimalApiResult<TValue>(
        this Result<SuccessWithWarnings<TValue>, DomainError> result,
        string? correlationId = null)
    {
        return result.Match(
            onSuccess: successWithWarnings =>
            {
                if (successWithWarnings.HasWarnings)
                {
                    return Results.Ok(ApiResponse<TValue>.Warning(
                        data: successWithWarnings.Value,
                        warnings: successWithWarnings.Warnings!.Value.ToApiErrors(),
                        correlationId: correlationId
                    ));
                }

                return Results.Ok(ApiResponse<TValue>.Success(successWithWarnings.Value, correlationId: correlationId));
            },
            onFailure: error => error.ToApiResponse<TValue>(correlationId).ToMinimalApiResult()
        );
    }

    #endregion

    #region Async Minimal API Results

    public static async Task<IResult> ToMinimalApiResultAsync<TValue>(
        this Task<Result<TValue, DomainError>> resultTask,
        string? correlationId = null)
    {
        var result = await resultTask;
        return result.ToMinimalApiResult(correlationId);
    }

    public static async Task<IResult> ToMinimalApiResultAsync<TValue>(
        this Task<Result<TValue, DomainErrors>> resultTask,
        string? correlationId = null)
    {
        var result = await resultTask;
        return result.ToMinimalApiResult(correlationId);
    }

    public static async Task<IResult> ToMinimalApiResultAsync<TValue>(
        this Task<Result<SuccessWithWarnings<TValue>, DomainError>> resultTask,
        string? correlationId = null)
    {
        var result = await resultTask;
        return result.ToMinimalApiResult(correlationId);
    }

    #endregion

    #region Problem Details Conversion

    public static ProblemDetails ToProblemDetails(this DomainErrors errors)
    {
        var mostSevereError = errors.GetMostSevereErrorType();
        return new ProblemDetails
        {
            Title = "Multiple errors occurred",
            Detail = errors.GetAggregateErrorMessage(),
            Status = (int)mostSevereError.ToHttpStatusCode(),
            Extensions = errors.ToErrorDetails() ?? new Dictionary<string, object>()
        };
    }

    public static ProblemDetails ToProblemDetails(
        this DomainError error,
        string? instance = null,
        Dictionary<string, object>? extensions = null)
    {
        var details = new ProblemDetails
        {
            Title = error.Code,
            Detail = error.Message,
            Status = (int)error.Type.ToHttpStatusCode(),
            Instance = instance
        };

        if (error.Target != null)
        {
            details.Extensions["target"] = error.Target;
        }

        if (error.Details != null)
        {
            foreach (var kvp in error.Details)
            {
                details.Extensions[kvp.Key] = kvp.Value;
            }
        }

        if (extensions != null)
        {
            foreach (var kvp in extensions)
            {
                details.Extensions[kvp.Key] = kvp.Value;
            }
        }

        return details;
    }

    #endregion

    #region Helper Methods

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
        DomainWarnings warnings,
        string? correlationId)
    {
        var response = ApiResponse<TValue>.Warning(
            data: value,
            warnings: warnings.ToApiErrors(),
            correlationId: correlationId,
            statusCode: HttpStatusCode.OK
        );
        return new OkObjectResult(response);
    }

    #endregion
}
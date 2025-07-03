using Acontplus.Core.Domain.Common;
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

    // Minimal API support
    public static IResult ToMinimalApiResult<TValue>(
        this Result<TValue, DomainError> result,
        string? correlationId = null)
    {
        return result.Match(
            onSuccess: value => TypedResults.Ok(ApiResponse<TValue>.Success(value, correlationId: correlationId)),
            onFailure: error => error.ToApiResponse<TValue>(correlationId).ToMinimalApiResult()
        );
    }

    public static IResult ToMinimalApiResult<TValue>(
        this Result<TValue, DomainErrors> result,
        string? correlationId = null)
    {
        return result.Match(
            onSuccess: value => TypedResults.Ok(ApiResponse<TValue>.Success(value, correlationId: correlationId)),
            onFailure: errors => errors.ToApiResponse<TValue>(correlationId).ToMinimalApiResult()
        );
    }

    // Helper methods
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
}
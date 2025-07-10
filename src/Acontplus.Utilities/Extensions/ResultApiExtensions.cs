namespace Acontplus.Utilities.Extensions;

public static class ResultApiExtensions
{
    #region Configuration

    private static Action<ApiResponse>? ConfigureResponse { get; set; }

    public static void ConfigureApiResponses(Action<ApiResponse> configureAction)
    {
        ConfigureResponse = configureAction;
    }

    #endregion

    #region Action Results (Controller-style)

    public static IActionResult ToActionResult<TValue>(
        this Result<TValue, DomainError> result,
        string? correlationId = null)
    {
        return result.Match<IActionResult>(
            value => CreateSuccessResponse(value, correlationId),
            error => error.ToApiResponse<TValue>(correlationId).ToActionResult()
        );
    }

    public static IActionResult ToActionResult<TValue>(
        this Result<TValue, DomainErrors> result,
        string? correlationId = null)
    {
        return result.Match<IActionResult>(
            value => CreateSuccessResponse(value, correlationId),
            errors => errors.ToApiResponse<TValue>(correlationId).ToActionResult()
        );
    }

    public static IActionResult ToActionResult<TValue>(
        this Result<SuccessWithWarnings<TValue>, DomainError> result,
        string? correlationId = null)
    {
        return result.Match<IActionResult>(
            successWithWarnings => successWithWarnings.ToActionResult(correlationId),
            error => error.ToApiResponse<TValue>(correlationId).ToActionResult()
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

    public static IResult ToMinimalApiResult<TValue>(
        this Result<TValue, DomainError> result,
        string? correlationId = null)
    {
        return result.Match<IResult>(
            value => CreateSuccessResult(value, correlationId),
            error => error.ToApiResponse<TValue>(correlationId).ToMinimalApiResult()
        );
    }

    public static IResult ToMinimalApiResult<TValue>(
        this Result<TValue, DomainErrors> result,
        string? correlationId = null)
    {
        return result.Match<IResult>(
            value => CreateSuccessResult(value, correlationId),
            errors => errors.ToApiResponse<TValue>(correlationId).ToMinimalApiResult()
        );
    }

    public static IResult ToMinimalApiResult<TValue>(
        this Result<SuccessWithWarnings<TValue>, DomainError> result,
        string? correlationId = null)
    {
        return result.Match<IResult>(
            successWithWarnings => successWithWarnings.ToMinimalApiResult(correlationId),
            error => error.ToApiResponse<TValue>(correlationId).ToMinimalApiResult()
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

    #region SuccessWithWarnings Extensions

    public static IActionResult ToActionResult<TValue>(
        this SuccessWithWarnings<TValue> successWithWarnings,
        string? correlationId = null)
    {
        return successWithWarnings.HasWarnings
            ? CreateWarningResponse(
                successWithWarnings.Value,
                successWithWarnings.Warnings!.Value,
                correlationId)
            : CreateSuccessResponse(successWithWarnings.Value, correlationId);
    }

    public static IResult ToMinimalApiResult<TValue>(
        this SuccessWithWarnings<TValue> successWithWarnings,
        string? correlationId = null)
    {
        return successWithWarnings.HasWarnings
            ? CreateWarningResult(
                successWithWarnings.Value,
                successWithWarnings.Warnings!.Value,
                correlationId)
            : CreateSuccessResult(successWithWarnings.Value, correlationId);
    }

    #endregion

    #region Helper Methods

    private static IActionResult CreateSuccessResponse<TValue>(TValue value, string? correlationId)
    {
        var response = ApiResponse<TValue>.Success(
            data: value,
            new ApiResponseOptions
            {
                CorrelationId = correlationId,
                StatusCode = HttpStatusCode.OK
            }
        );
        ConfigureResponse?.Invoke(response.ToBaseResponse());
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
            new ApiResponseOptions
            {
                CorrelationId = correlationId,
                StatusCode = HttpStatusCode.OK
            }
        );
        ConfigureResponse?.Invoke(response.ToBaseResponse());
        return new OkObjectResult(response);
    }

    private static IResult CreateSuccessResult<TValue>(TValue value, string? correlationId)
    {
        var response = ApiResponse<TValue>.Success(
            data: value,
            new ApiResponseOptions
            {
                CorrelationId = correlationId,
                StatusCode = HttpStatusCode.OK
            }
        );
        ConfigureResponse?.Invoke(response.ToBaseResponse());
        return TypedResults.Ok(response);
    }

    private static IResult CreateWarningResult<TValue>(
        TValue value,
        DomainWarnings warnings,
        string? correlationId)
    {
        var response = ApiResponse<TValue>.Warning(
            data: value,
            warnings: warnings.ToApiErrors(),
            new ApiResponseOptions
            {
                CorrelationId = correlationId,
                StatusCode = HttpStatusCode.OK
            }
        );
        ConfigureResponse?.Invoke(response.ToBaseResponse());
        return TypedResults.Ok(response);
    }

    #endregion
}
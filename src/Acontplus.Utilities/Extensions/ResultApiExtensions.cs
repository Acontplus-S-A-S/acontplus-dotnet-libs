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

    #region CRUD-Specific Extensions (Synchronous)

    // GET: 200 OK or 204 NoContent
    public static IActionResult ToGetActionResult<T>(this Result<T, DomainError> result)
    {
        return result.Match<IActionResult>(
            value => value is null ? new NoContentResult() : new OkObjectResult(value),
            error => error.ToApiResponse<T>(null, null).ToActionResult()
        );
    }

    // POST: 201 Created (with Location header)
    public static IActionResult ToCreatedActionResult<T>(this Result<T, DomainError> result, string locationUri)
    {
        return result.Match<IActionResult>(
            value => new CreatedResult(locationUri, value),
            error => error.ToApiResponse<T>(null, null).ToActionResult()
        );
    }

    // PUT: 200 OK or 204 NoContent
    public static IActionResult ToPutActionResult<T>(this Result<T, DomainError> result)
    {
        return result.Match<IActionResult>(
            value => value is null ? new NoContentResult() : new OkObjectResult(value),
            error => error.ToApiResponse<T>(null, null).ToActionResult()
        );
    }

    // DELETE: 204 NoContent or 404 NotFound
    public static IActionResult ToDeleteActionResult(this Result<bool, DomainError> result)
    {
        return result.Match<IActionResult>(
            deleted => deleted ? new NoContentResult() : new NotFoundResult(),
            error => error.ToApiResponse<bool>(null, null).ToActionResult()
        );
    }

    // Minimal API: GET: 200 OK or 204 NoContent
    public static IResult ToGetMinimalApiResult<T>(this Result<T, DomainError> result)
    {
        return result.Match<IResult>(
            value => value is null ? TypedResults.NoContent() : TypedResults.Ok(value),
            error => TypedResults.Problem(error.Message)
        );
    }

    // Minimal API: POST: 201 Created (with Location header)
    public static IResult ToCreatedMinimalApiResult<T>(this Result<T, DomainError> result, string locationUri)
    {
        return result.Match<IResult>(
            value => TypedResults.Created(locationUri, value),
            error => TypedResults.Problem(error.Message)
        );
    }

    // Minimal API: PUT: 200 OK or 204 NoContent
    public static IResult ToPutMinimalApiResult<T>(this Result<T, DomainError> result)
    {
        return result.Match<IResult>(
            value => value is null ? TypedResults.NoContent() : TypedResults.Ok(value),
            error => TypedResults.Problem(error.Message)
        );
    }

    // Minimal API: DELETE: 204 NoContent or 404 NotFound
    public static IResult ToDeleteMinimalApiResult(this Result<bool, DomainError> result)
    {
        return result.Match<IResult>(
            deleted => deleted ? TypedResults.NoContent() : TypedResults.NotFound(),
            error => TypedResults.Problem(error.Message)
        );
    }

    #endregion

    #region CRUD-Specific Extensions (Async)

    // GET: 200 OK or 204 NoContent (async)
    public static async Task<IActionResult> ToGetActionResultAsync<T>(this Task<Result<T, DomainError>> resultTask)
    {
        var result = await resultTask;
        return result.ToGetActionResult();
    }

    // POST: 201 Created (async)
    public static async Task<IActionResult> ToCreatedActionResultAsync<T>(this Task<Result<T, DomainError>> resultTask, string locationUri)
    {
        var result = await resultTask;
        return result.ToCreatedActionResult(locationUri);
    }

    // PUT: 200 OK or 204 NoContent (async)
    public static async Task<IActionResult> ToPutActionResultAsync<T>(this Task<Result<T, DomainError>> resultTask)
    {
        var result = await resultTask;
        return result.ToPutActionResult();
    }

    // DELETE: 204 NoContent or 404 NotFound (async)
    public static async Task<IActionResult> ToDeleteActionResultAsync(this Task<Result<bool, DomainError>> resultTask)
    {
        var result = await resultTask;
        return result.ToDeleteActionResult();
    }

    // Minimal API: GET: 200 OK or 204 NoContent (async)
    public static async Task<IResult> ToGetMinimalApiResultAsync<T>(this Task<Result<T, DomainError>> resultTask)
    {
        var result = await resultTask;
        return result.ToGetMinimalApiResult();
    }

    // Minimal API: POST: 201 Created (async)
    public static async Task<IResult> ToCreatedMinimalApiResultAsync<T>(this Task<Result<T, DomainError>> resultTask, string locationUri)
    {
        var result = await resultTask;
        return result.ToCreatedMinimalApiResult(locationUri);
    }

    // Minimal API: PUT: 200 OK or 204 NoContent (async)
    public static async Task<IResult> ToPutMinimalApiResultAsync<T>(this Task<Result<T, DomainError>> resultTask)
    {
        var result = await resultTask;
        return result.ToPutMinimalApiResult();
    }

    // Minimal API: DELETE: 204 NoContent or 404 NotFound (async)
    public static async Task<IResult> ToDeleteMinimalApiResultAsync(this Task<Result<bool, DomainError>> resultTask)
    {
        var result = await resultTask;
        return result.ToDeleteMinimalApiResult();
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
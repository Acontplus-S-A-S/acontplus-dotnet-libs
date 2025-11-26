using Acontplus.Core.Domain.Common.Results;
using Acontplus.Core.Domain.Enums;
using Acontplus.Core.Domain.Exceptions;
using Acontplus.Persistence.Common.Exceptions;
using Acontplus.TestApi.Services;
using Acontplus.Utilities.Extensions;

namespace Acontplus.TestApi.Controllers.Demo;

[ApiController]
[Route("api/[controller]")]
public class BusinessExceptionTestController : ControllerBase
{
    private readonly IBusinessExceptionTestService _service;
    private readonly ILogger<BusinessExceptionTestController> _logger;

    public BusinessExceptionTestController(
        IBusinessExceptionTestService service,
        ILogger<BusinessExceptionTestController> logger)
    {
        _service = service;
        _logger = logger;
    }

    #region Direct Service Calls - Result pattern

    [HttpPost("validation-from-service")]
    public async Task<IActionResult> ValidationFromService()
    {
        _logger.LogInformation("Calling service that may throw validation exception");

        var result = await _service.ValidateEmailAsync("invalid-email");
        return result.ToActionResult("Validation completed");
    }

    [HttpGet("not-found-from-service/{id}")]
    public async Task<IActionResult> NotFoundFromService(int id)
    {
        _logger.LogInformation("Calling service to get customer ID: {Id}", id);

        var result = await _service.GetCustomerAsync(id);
        return result.ToActionResult("Customer retrieved successfully");
    }

    [HttpPost("conflict-from-service")]
    public async Task<IActionResult> ConflictFromService([FromBody] CustomerRequest request)
    {
        _logger.LogInformation("Calling service to create customer with email: {Email}", request.Email);

        var result = await _service.CreateCustomerAsync(request.Email);
        return result.ToActionResult("Customer created successfully");
    }

    [HttpPost("sql-error-from-service")]
    public async Task<IActionResult> SqlErrorFromService()
    {
        _logger.LogInformation("Calling service to execute database operation");

        var result = await _service.ExecuteDatabaseOperationAsync();
        return result.ToActionResult("Database operation completed");
    }

    [HttpGet("internal-error-from-service")]
    public async Task<IActionResult> InternalErrorFromService()
    {
        _logger.LogInformation("Calling service to process complex operation");

        var result = await _service.ProcessComplexOperationAsync();
        return result.ToActionResult("Operation completed");
    }

    #endregion

    #region Wrapped Service Calls - Result pattern responses

    [HttpGet("wrapped-exception/{id}")]
    public async Task<IActionResult> WrappedException(int id)
    {
        _logger.LogInformation("Calling service with try-catch wrapper");

        var result = await _service.GetCustomerAsync(id);
        return result.ToActionResult("Customer retrieved successfully");
    }

    [HttpPost("wrapped-with-context")]
    public async Task<IActionResult> WrappedWithContext([FromBody] CustomerRequest request)
    {
        _logger.LogInformation("Calling service and wrapping exception with context");

        var result = await _service.CreateCustomerAsync(request.Email);
        return result.ToActionResult("Customer created successfully");
    }

    #endregion

    #region Multiple Layers - Deep Call Stack

    [HttpGet("deep-call-stack/{id}")]
    public async Task<IActionResult> DeepCallStack(int id)
    {
        _logger.LogInformation("Testing deep call stack exception propagation");

        var result = await _service.GetCustomerWithDeepStackAsync(id);
        return result.ToActionResult("Customer retrieved successfully");
    }

    #endregion

    #region Async Exception Handling

    [HttpPost("async-exception")]
    public async Task<IActionResult> AsyncException()
    {
        _logger.LogInformation("Testing async exception handling");

        var result = await _service.AsyncOperationThatFailsAsync();
        return result.ToActionResult("Async operation completed");
    }

    [HttpGet("task-run-exception")]
    public async Task<IActionResult> TaskRunException()
    {
        _logger.LogInformation("Testing exception from Task.Run");

        var result = await _service.TaskRunOperationAsync();
        return result.ToActionResult("Background task completed");
    }

    #endregion

    #region Success Cases - Using ResultApiExtensions

    [HttpGet("success/{id}")]
    public async Task<IActionResult> Success(int id)
    {
        var result = await _service.GetValidCustomerAsync(id);
        return result.ToActionResult("Customer retrieved successfully");
    }

    [HttpGet("success-with-correlation/{id}")]
    public async Task<IActionResult> SuccessWithCorrelation(int id)
    {
        var result = await _service.GetValidCustomerAsync(id);
        var correlationId = HttpContext.TraceIdentifier;
        return result.ToActionResult("Customer retrieved successfully", correlationId);
    }

    [HttpGet("success-async/{id}")]
    public Task<IActionResult> SuccessAsync(int id)
    {
        var task = _service.GetValidCustomerAsync(id);
        return task.ToActionResultAsync("Customer retrieved successfully");
    }

    [HttpGet("crud/get/{id}")]
    public async Task<IActionResult> CrudGet(int id)
    {
        var result = id <= 100
            ? await _service.GetValidCustomerAsync(id)
            : Result<CustomerModel?, DomainError>.Success(null);

        return result.ToGetActionResult();
    }

    [HttpPost("crud/create")]
    public async Task<IActionResult> CrudCreate([FromBody] CustomerRequest request)
    {
        var result = await _service.CreateCustomerAsync(request.Email);
        var locationUri = $"/api/BusinessExceptionTest/crud/get/{result.Match(v => v.Id, _ => 0)}";
        return result.ToCreatedActionResult(locationUri);
    }

    [HttpPut("crud/update/{id}")]
    public async Task<IActionResult> CrudUpdate(int id, [FromBody] CustomerRequest request)
    {
        var result = await _service.GetValidCustomerAsync(id);
        // If successful, update values in returned object
        return result.Match<IActionResult>(
            success: value =>
            {
                value.Email = request.Email;
                value.Name = request.Name;
                return Result<CustomerModel, DomainError>.Success(value).ToPutActionResult();
            },
            failure: error => Result<CustomerModel, DomainError>.Failure(error).ToActionResult());
    }

    [HttpDelete("crud/delete/{id}")]
    public async Task<IActionResult> CrudDelete(int id)
    {
        var result = id <= 100
            ? Result<bool, DomainError>.Success(true)
            : Result<bool, DomainError>.Failure(DomainError.NotFound("CUSTOMER_NOT_FOUND", $"Customer {id} not found"));

        return result.ToDeleteActionResult();
    }

    [HttpPost("success-with-warnings")]
    public async Task<IActionResult> SuccessWithWarnings([FromBody] CustomerRequest request)
    {
        var result = await _service.GetValidCustomerAsync(1);
        return result.Match<IActionResult>(
            success: value =>
            {
                var warnings = new List<DomainError>
                {
                    DomainError.Validation("EMAIL_FORMAT_WARNING", "Email format is valid but non-standard", "email"),
                    DomainError.Validation("NAME_LENGTH_WARNING", "Name is very short", "name")
                };

                var domainWarnings = DomainWarnings.Multiple(warnings);
                var successWithWarnings = new SuccessWithWarnings<CustomerModel>(value, domainWarnings);
                return Result<SuccessWithWarnings<CustomerModel>, DomainError>.Success(successWithWarnings).ToActionResult("Customer created with warnings");
            },
            failure: error => Result<CustomerModel, DomainError>.Failure(error).ToActionResult());
    }

    [HttpGet("custom-response/{id}")]
    public async Task<IActionResult> CustomResponse(int id)
    {
        var result = await _service.GetValidCustomerAsync(id);
        return result.Match<IActionResult>(
            success: value =>
            {
                var response = ApiResponse<CustomerModel>.Success(
                    value,
                    new ApiResponseOptions
                    {
                        Message = "Customer retrieved with custom response",
                        CorrelationId = HttpContext.TraceIdentifier,
                        StatusCode = System.Net.HttpStatusCode.OK,
                        Metadata = new Dictionary<string, object>
                        {
                            ["requestTime"] = DateTime.UtcNow,
                            ["version"] = "1.0",
                            ["customField"] = "Custom value"
                        }
                    });

                return new OkObjectResult(response);
            },
            failure: error => Result<CustomerModel, DomainError>.Failure(error).ToActionResult());
    }

    #endregion

    #region Comparison: Exception vs Result Pattern

    [HttpGet("compare/exception/{id}")]
    public async Task<IActionResult> CompareException(int id)
    {
        var result = await _service.GetCustomerAsync(id);
        return result.ToActionResult("Customer retrieved successfully");
    }

    [HttpGet("compare/result/{id}")]
    public async Task<IActionResult> CompareResult(int id)
    {
        var result = await _service.GetCustomerAsync(id);
        return result.ToActionResult("Customer found");
    }

    #endregion

    #region Info Endpoint

    [HttpGet("info")]
    public IActionResult GetInfo()
    {
        return Ok(new
        {
            description = "Business Layer Exception Testing with ResultApiExtensions",
            purpose = "Demonstrate exception handling and Result pattern with standardized API responses",
            testCategories = new
            {
                exceptionExamples = new[]
                {
                    new { endpoint = "POST /validation-from-service", description = "Service throws ValidationException" },
                    new { endpoint = "GET /not-found-from-service/{id}", description = "Service throws NotFound DomainException" },
                    new { endpoint = "POST /conflict-from-service", description = "Service throws Conflict DomainException" },
                    new { endpoint = "POST /sql-error-from-service", description = "Service throws SqlDomainException" },
                    new { endpoint = "GET /internal-error-from-service", description = "Service throws Internal DomainException" }
                },
                wrappedExceptions = new[]
                {
                    new { endpoint = "GET /wrapped-exception/{id}", description = "Controller catches and re-throws" },
                    new { endpoint = "POST /wrapped-with-context", description = "Controller wraps exception with context" }
                },
                successWithResultPattern = new[]
                {
                    new { endpoint = "GET /success/{id}", description = "Success with ToActionResult" },
                    new { endpoint = "GET /success-with-correlation/{id}", description = "Success with correlation ID" },
                    new { endpoint = "GET /success-async/{id}", description = "Async success with ToActionResultAsync" },
                    new { endpoint = "POST /success-with-warnings", description = "Success with warnings" },
                    new { endpoint = "GET /custom-response/{id}", description = "Custom ApiResponse" }
                },
                crudPatterns = new[]
                {
                    new { endpoint = "GET /crud/get/{id}", description = "GET: 200 OK or 204 NoContent using ToGetActionResult" },
                    new { endpoint = "POST /crud/create", description = "POST: 201 Created using ToCreatedActionResult" },
                    new { endpoint = "PUT /crud/update/{id}", description = "PUT: 200 OK using ToPutActionResult" },
                    new { endpoint = "DELETE /crud/delete/{id}", description = "DELETE: 204 NoContent or 404 NotFound using ToDeleteActionResult" }
                },
                comparison = new[]
                {
                    new { endpoint = "GET /compare/exception/{id}", description = "Exception approach (throws)" },
                    new { endpoint = "GET /compare/result/{id}", description = "Result pattern approach (catches and converts)" }
                },
                deepCallStack = new[]
                {
                    new { endpoint = "GET /deep-call-stack/{id}", description = "Exception from deep service layer" }
                },
                asyncHandling = new[]
                {
                    new { endpoint = "POST /async-exception", description = "Async method exception" },
                    new { endpoint = "GET /task-run-exception", description = "Task.Run exception" }
                }
            },
            resultApiExtensionMethods = new
            {
                basic = new[]
                {
                    "ToActionResult() - Basic conversion with default message",
                    "ToActionResult(message) - With custom success message",
                    "ToActionResult(message, correlationId) - With message and correlation ID",
                    "ToActionResultAsync() - Async version"
                },
                crud = new[]
                {
                    "ToGetActionResult() - 200 OK with data or 204 NoContent if null",
                    "ToCreatedActionResult(locationUri) - 201 Created with Location header",
                    "ToPutActionResult() - 200 OK with data or 204 NoContent",
                    "ToDeleteActionResult() - 204 NoContent on success or 404 NotFound"
                },
                advanced = new[]
                {
                    "SuccessWithWarnings - Include warnings in successful response",
                    "ApiResponse.Success() - Full control over response structure",
                    "Custom metadata - Add custom fields to response"
                }
            },
            responseFormats = new
            {
                success = new
                {
                    format = "ApiResponse<T>",
                    example = new
                    {
                        success = true,
                        code = "200",
                        message = "Customer retrieved successfully",
                        data = new { id = 1, name = "John Doe", email = "john@example.com", status = "Active" },
                        correlationId = "abc-123-def",
                        timestamp = "2024-01-15T10:30:00Z"
                    }
                },
                error = new
                {
                    format = "ApiResponse",
                    example = new
                    {
                        success = false,
                        code = "404",
                        message = "Customer not found",
                        errors = new[]
                        {
                            new
                            {
                                code = "CUSTOMER_NOT_FOUND",
                                message = "Customer not found",
                                category = "business",
                                severity = "warning"
                            }
                        },
                        correlationId = "abc-123-def",
                        timestamp = "2024-01-15T10:30:00Z"
                    }
                },
                successWithWarnings = new
                {
                    format = "ApiResponse<T> with warnings",
                    example = new
                    {
                        success = true,
                        code = "200",
                        message = "Customer created with warnings",
                        data = new { id = 1, name = "J", email = "j@example.com" },
                        warnings = new[]
                        {
                            new
                            {
                                code = "NAME_LENGTH_WARNING",
                                message = "Name is very short",
                                target = "name",
                                category = "validation",
                                severity = "warning"
                            }
                        },
                        correlationId = "abc-123-def"
                    }
                }
            },
            notes = new[]
            {
                "All exceptions thrown from services are caught by ApiExceptionMiddleware",
                "ResultApiExtensions provide consistent response format for success cases",
                "CRUD methods (ToGetActionResult, ToCreatedActionResult, etc.) handle REST semantics automatically",
                "DomainException types preserve their ErrorType and ErrorCode",
                "Both exception throwing and Result pattern produce identical response formats",
                "Use exception throwing for simpler code, Result pattern for expected failures"
            }
        });
    }

    #endregion
}

/// <summary>
/// Request model for customer operations.
/// </summary>
public class CustomerRequest
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Customer model for responses.
/// </summary>
public class CustomerModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
}

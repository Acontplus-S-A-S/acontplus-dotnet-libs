using Acontplus.Core.Domain.Common.Results;
using Acontplus.Core.Domain.Enums;
using Acontplus.Core.Domain.Exceptions;
using Acontplus.Persistence.Common.Exceptions;
using Acontplus.Utilities.Extensions;

namespace Acontplus.TestApi.Controllers.Demo;

/// <summary>
/// Controller for testing exception handling middleware.
/// Simulates various exception scenarios to demonstrate how ApiExceptionMiddleware handles different error types.
/// Also demonstrates the Result pattern as an alternative to throwing exceptions.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ExceptionTestController : ControllerBase
{
    private readonly ILogger<ExceptionTestController> _logger;

    public ExceptionTestController(ILogger<ExceptionTestController> logger)
    {
        _logger = logger;
    }

    #region Client Errors (4xx) - DomainException Examples

    /// <summary>
    /// Simulates a validation error (422 Unprocessable Entity).
    /// Example: Invalid input data that fails business rules.
    /// </summary>
    [HttpPost("validation-error")]
    public IActionResult ThrowValidationError()
    {
        _logger.LogInformation("Simulating validation error");
        
        throw new GenericDomainException(
            ErrorType.Validation,
            "INVALID_EMAIL",
            "The email address format is invalid");
    }

    /// <summary>
    /// Simulates a validation error using Result pattern (422 Unprocessable Entity).
    /// Example: Invalid input data that fails business rules - Result pattern approach.
    /// </summary>
    [HttpPost("validation-error-result")]
    public IActionResult ValidationErrorResult()
    {
        _logger.LogInformation("Simulating validation error using Result pattern");
        
        var error = DomainError.Validation(
            "INVALID_EMAIL",
            "The email address format is invalid",
            "email");
        
        var result = Result<object, DomainError>.Failure(error);
        return result.ToActionResult();
    }

    /// <summary>
    /// Simulates a bad request error (400 Bad Request).
    /// Example: Malformed request syntax.
    /// </summary>
    [HttpPost("bad-request")]
    public IActionResult ThrowBadRequest()
    {
        _logger.LogInformation("Simulating bad request error");
        
        throw new GenericDomainException(
            ErrorType.BadRequest,
            "INVALID_JSON",
            "The request body contains invalid JSON syntax");
    }

    /// <summary>
    /// Simulates a bad request using Result pattern (400 Bad Request).
    /// </summary>
    [HttpPost("bad-request-result")]
    public IActionResult BadRequestResult()
    {
        _logger.LogInformation("Simulating bad request using Result pattern");
        
        var error = DomainError.BadRequest(
            "INVALID_JSON",
            "The request body contains invalid JSON syntax");
        
        var result = Result<object, DomainError>.Failure(error);
        return result.ToActionResult();
    }

    /// <summary>
    /// Simulates a not found error (404 Not Found).
    /// Example: Resource doesn't exist in the system.
    /// </summary>
    [HttpGet("not-found/{id}")]
    public IActionResult ThrowNotFound(int id)
    {
        _logger.LogInformation("Simulating not found error for ID: {Id}", id);
        
        throw new GenericDomainException(
            ErrorType.NotFound,
            "CUSTOMER_NOT_FOUND",
            $"Customer with ID {id} was not found");
    }

    /// <summary>
    /// Simulates a not found error using Result pattern (404 Not Found).
    /// </summary>
    [HttpGet("not-found-result/{id}")]
    public IActionResult NotFoundResult(int id)
    {
        _logger.LogInformation("Simulating not found using Result pattern for ID: {Id}", id);
        
        var error = DomainError.NotFound(
            "CUSTOMER_NOT_FOUND",
            $"Customer with ID {id} was not found",
            "customerId");
        
        var result = Result<object, DomainError>.Failure(error);
        return result.ToActionResult();
    }

    /// <summary>
    /// Simulates a conflict error (409 Conflict).
    /// Example: Duplicate resource or concurrent modification.
    /// </summary>
    [HttpPost("conflict")]
    public IActionResult ThrowConflict()
    {
        _logger.LogInformation("Simulating conflict error");
        
        throw new GenericDomainException(
            ErrorType.Conflict,
            "DUPLICATE_EMAIL",
            "A user with this email address already exists");
    }

    /// <summary>
    /// Simulates a conflict using Result pattern (409 Conflict).
    /// </summary>
    [HttpPost("conflict-result")]
    public IActionResult ConflictResult()
    {
        _logger.LogInformation("Simulating conflict using Result pattern");
        
        var error = DomainError.Conflict(
            "DUPLICATE_EMAIL",
            "A user with this email address already exists",
            "email");
        
        var result = Result<object, DomainError>.Failure(error);
        return result.ToActionResult();
    }

    /// <summary>
    /// Simulates an unauthorized error (401 Unauthorized).
    /// Example: Missing or invalid authentication token.
    /// </summary>
    [HttpGet("unauthorized")]
    public IActionResult ThrowUnauthorized()
    {
        _logger.LogInformation("Simulating unauthorized error");
        
        throw new GenericDomainException(
            ErrorType.Unauthorized,
            "INVALID_TOKEN",
            "The authentication token is invalid or expired");
    }

    /// <summary>
    /// Simulates an unauthorized using Result pattern (401 Unauthorized).
    /// </summary>
    [HttpGet("unauthorized-result")]
    public IActionResult UnauthorizedResult()
    {
        _logger.LogInformation("Simulating unauthorized using Result pattern");
        
        var error = DomainError.Unauthorized(
            "INVALID_TOKEN",
            "The authentication token is invalid or expired");
        
        var result = Result<object, DomainError>.Failure(error);
        return result.ToActionResult();
    }

    /// <summary>
    /// Simulates a forbidden error (403 Forbidden).
    /// Example: User lacks required permissions.
    /// </summary>
    [HttpGet("forbidden")]
    public IActionResult ThrowForbidden()
    {
        _logger.LogInformation("Simulating forbidden error");
        
        throw new GenericDomainException(
            ErrorType.Forbidden,
            "INSUFFICIENT_PERMISSIONS",
            "You do not have permission to access this resource");
    }

    /// <summary>
    /// Simulates a forbidden using Result pattern (403 Forbidden).
    /// </summary>
    [HttpGet("forbidden-result")]
    public IActionResult ForbiddenResult()
    {
        _logger.LogInformation("Simulating forbidden using Result pattern");
        
        var error = DomainError.Forbidden(
            "INSUFFICIENT_PERMISSIONS",
            "You do not have permission to access this resource");
        
        var result = Result<object, DomainError>.Failure(error);
        return result.ToActionResult();
    }

    /// <summary>
    /// Simulates a rate limit error (429 Too Many Requests).
    /// Example: Too many requests from client.
    /// </summary>
    [HttpGet("rate-limited")]
    public IActionResult ThrowRateLimited()
    {
        _logger.LogInformation("Simulating rate limit error");
        
        throw new GenericDomainException(
            ErrorType.RateLimited,
            "RATE_LIMIT_EXCEEDED",
            "Rate limit exceeded. Please try again in 60 seconds");
    }

    /// <summary>
    /// Simulates rate limiting using Result pattern (429 Too Many Requests).
    /// </summary>
    [HttpGet("rate-limited-result")]
    public IActionResult RateLimitedResult()
    {
        _logger.LogInformation("Simulating rate limit using Result pattern");
        
        var error = DomainError.RateLimited(
            "RATE_LIMIT_EXCEEDED",
            "Rate limit exceeded. Please try again in 60 seconds",
            details: new Dictionary<string, object>
            {
                ["retryAfterSeconds"] = 60,
                ["limit"] = 100,
                ["resetTime"] = DateTime.UtcNow.AddSeconds(60)
            });
        
        var result = Result<object, DomainError>.Failure(error);
        return result.ToActionResult();
    }

    /// <summary>
    /// Simulates a payload too large error (413 Payload Too Large).
    /// Example: Request body exceeds size limit.
    /// </summary>
    [HttpPost("payload-too-large")]
    public IActionResult ThrowPayloadTooLarge()
    {
        _logger.LogInformation("Simulating payload too large error");
        
        throw new GenericDomainException(
            ErrorType.PayloadTooLarge,
            "PAYLOAD_TOO_LARGE",
            "The request body exceeds the maximum allowed size of 10MB");
    }

    #endregion

    #region Server Errors (5xx) - DomainException Examples

    /// <summary>
    /// Simulates an internal server error (500 Internal Server Error).
    /// Example: Unexpected system error.
    /// </summary>
    [HttpGet("internal-error")]
    public IActionResult ThrowInternalError()
    {
        _logger.LogInformation("Simulating internal server error");
        
        throw new GenericDomainException(
            ErrorType.Internal,
            "INTERNAL_ERROR",
            "An unexpected error occurred while processing your request");
    }

    /// <summary>
    /// Simulates an internal error using Result pattern (500 Internal Server Error).
    /// </summary>
    [HttpGet("internal-error-result")]
    public IActionResult InternalErrorResult()
    {
        _logger.LogInformation("Simulating internal error using Result pattern");
        
        var error = DomainError.Internal(
            "INTERNAL_ERROR",
            "An unexpected error occurred while processing your request");
        
        var result = Result<object, DomainError>.Failure(error);
        return result.ToActionResult();
    }

    /// <summary>
    /// Simulates a service unavailable error (503 Service Unavailable).
    /// Example: Service is temporarily down or under maintenance.
    /// </summary>
    [HttpGet("service-unavailable")]
    public IActionResult ThrowServiceUnavailable()
    {
        _logger.LogInformation("Simulating service unavailable error");
        
        throw new GenericDomainException(
            ErrorType.ServiceUnavailable,
            "SERVICE_UNAVAILABLE",
            "The service is temporarily unavailable. Please try again later");
    }

    /// <summary>
    /// Simulates service unavailable using Result pattern (503 Service Unavailable).
    /// </summary>
    [HttpGet("service-unavailable-result")]
    public IActionResult ServiceUnavailableResult()
    {
        _logger.LogInformation("Simulating service unavailable using Result pattern");
        
        var error = DomainError.ServiceUnavailable(
            "SERVICE_UNAVAILABLE",
            "The service is temporarily unavailable. Please try again later",
            details: new Dictionary<string, object>
            {
                ["estimatedDowntime"] = "5 minutes",
                ["maintenanceMode"] = true
            });
        
        var result = Result<object, DomainError>.Failure(error);
        return result.ToActionResult();
    }

    /// <summary>
    /// Simulates a gateway timeout error (504 Gateway Timeout).
    /// Example: External service timeout.
    /// </summary>
    [HttpGet("timeout")]
    public IActionResult ThrowTimeout()
    {
        _logger.LogInformation("Simulating timeout error");
        
        throw new GenericDomainException(
            ErrorType.Timeout,
            "EXTERNAL_SERVICE_TIMEOUT",
            "The external payment service did not respond within the expected time");
    }

    /// <summary>
    /// Simulates timeout using Result pattern (504 Gateway Timeout).
    /// </summary>
    [HttpGet("timeout-result")]
    public IActionResult TimeoutResult()
    {
        _logger.LogInformation("Simulating timeout using Result pattern");
        
        var error = DomainError.Timeout(
            "EXTERNAL_SERVICE_TIMEOUT",
            "The external payment service did not respond within the expected time",
            "paymentService",
            new Dictionary<string, object>
            {
                ["timeoutSeconds"] = 30,
                ["service"] = "PaymentGateway"
            });
        
        var result = Result<object, DomainError>.Failure(error);
        return result.ToActionResult();
    }

    /// <summary>
    /// Simulates a not implemented error (501 Not Implemented).
    /// Example: Feature not yet implemented.
    /// </summary>
    [HttpGet("not-implemented")]
    public IActionResult ThrowNotImplemented()
    {
        _logger.LogInformation("Simulating not implemented error");
        
        throw new GenericDomainException(
            ErrorType.NotImplemented,
            "FEATURE_NOT_IMPLEMENTED",
            "This feature is not yet implemented");
    }

    /// <summary>
    /// Simulates an external service error (502 Bad Gateway).
    /// Example: External API returned an error.
    /// </summary>
    [HttpGet("external-error")]
    public IActionResult ThrowExternalError()
    {
        _logger.LogInformation("Simulating external service error");
        
        throw new GenericDomainException(
            ErrorType.External,
            "EXTERNAL_API_ERROR",
            "The external payment gateway returned an error");
    }

    /// <summary>
    /// Simulates external error using Result pattern (502 Bad Gateway).
    /// </summary>
    [HttpGet("external-error-result")]
    public IActionResult ExternalErrorResult()
    {
        _logger.LogInformation("Simulating external error using Result pattern");
        
        var error = DomainError.External(
            "EXTERNAL_API_ERROR",
            "The external payment gateway returned an error",
            "paymentGateway",
            new Dictionary<string, object>
            {
                ["externalErrorCode"] = "GATEWAY_503",
                ["externalMessage"] = "Service temporarily unavailable"
            });
        
        var result = Result<object, DomainError>.Failure(error);
        return result.ToActionResult();
    }

    #endregion

    #region SQL Exception Examples

    /// <summary>
    /// Simulates a SQL foreign key constraint violation.
    /// Example: Trying to delete a record that is referenced by other records.
    /// </summary>
    [HttpDelete("sql/foreign-key-violation")]
    public IActionResult ThrowSqlForeignKeyViolation()
    {
        _logger.LogInformation("Simulating SQL foreign key violation");
        
        // Create a SqlErrorInfo using record positional parameters
        var sqlErrorInfo = new SqlErrorInfo(
            ErrorType.Conflict,
            "FK_VIOLATION",
            "Cannot delete customer because it has associated orders",
            new InvalidOperationException("SQL Server Error 547: Foreign Key Violation")
        );
        
        throw new SqlDomainException(sqlErrorInfo);
    }

    /// <summary>
    /// Simulates a SQL unique constraint violation.
    /// Example: Trying to insert a duplicate record.
    /// </summary>
    [HttpPost("sql/unique-violation")]
    public IActionResult ThrowSqlUniqueViolation()
    {
        _logger.LogInformation("Simulating SQL unique constraint violation");
        
        var sqlErrorInfo = new SqlErrorInfo(
            ErrorType.Conflict,
            "UNIQUE_VIOLATION",
            "A record with this email address already exists",
            new InvalidOperationException("SQL Server Error 2627: Unique Constraint Violation")
        );
        
        throw new SqlDomainException(sqlErrorInfo);
    }

    /// <summary>
    /// Simulates a SQL timeout error.
    /// Example: Database query took too long to execute.
    /// </summary>
    [HttpGet("sql/timeout")]
    public IActionResult ThrowSqlTimeout()
    {
        _logger.LogInformation("Simulating SQL timeout error");
        
        var sqlErrorInfo = new SqlErrorInfo(
            ErrorType.Timeout,
            "SQL_TIMEOUT",
            "The database operation timed out after 30 seconds",
            new TimeoutException("SQL Server Timeout")
        );
        
        throw new SqlDomainException(sqlErrorInfo);
    }

    /// <summary>
    /// Simulates a SQL deadlock error.
    /// Example: Two queries waiting for each other's locks.
    /// </summary>
    [HttpPost("sql/deadlock")]
    public IActionResult ThrowSqlDeadlock()
    {
        _logger.LogInformation("Simulating SQL deadlock error");
        
        var sqlErrorInfo = new SqlErrorInfo(
            ErrorType.Conflict,
            "SQL_DEADLOCK",
            "A database deadlock occurred. The transaction has been rolled back",
            new InvalidOperationException("SQL Server Error 1205: Deadlock")
        );
        
        throw new SqlDomainException(sqlErrorInfo);
    }

    /// <summary>
    /// Simulates a SQL connection error.
    /// Example: Cannot connect to database server.
    /// </summary>
    [HttpGet("sql/connection-error")]
    public IActionResult ThrowSqlConnectionError()
    {
        _logger.LogInformation("Simulating SQL connection error");
        
        var sqlErrorInfo = new SqlErrorInfo(
            ErrorType.ServiceUnavailable,
            "SQL_CONNECTION_FAILED",
            "Unable to connect to the database server",
            new InvalidOperationException("SQL Server Error: Connection Failed")
        );
        
        throw new SqlDomainException(sqlErrorInfo);
    }

    #endregion

    #region Standard .NET Exceptions

    /// <summary>
    /// Simulates a standard ArgumentNullException.
    /// Example: Required parameter is null.
    /// </summary>
    [HttpPost("standard/argument-null")]
    public IActionResult ThrowArgumentNull()
    {
        _logger.LogInformation("Simulating ArgumentNullException");
        
        throw new ArgumentNullException("customerId", "Customer ID cannot be null");
    }

    /// <summary>
    /// Simulates a standard ArgumentException.
    /// Example: Invalid argument value.
    /// </summary>
    [HttpPost("standard/argument-invalid")]
    public IActionResult ThrowArgumentInvalid()
    {
        _logger.LogInformation("Simulating ArgumentException");
        
        throw new ArgumentException("Age must be between 0 and 150", "age");
    }

    /// <summary>
    /// Simulates a standard InvalidOperationException.
    /// Example: Operation is not valid in current state.
    /// </summary>
    [HttpPost("standard/invalid-operation")]
    public IActionResult ThrowInvalidOperation()
    {
        _logger.LogInformation("Simulating InvalidOperationException");
        
        throw new InvalidOperationException("Cannot process payment for an order that is already cancelled");
    }

    /// <summary>
    /// Simulates a standard TimeoutException.
    /// Example: Operation exceeded time limit.
    /// </summary>
    [HttpGet("standard/timeout")]
    public IActionResult ThrowStandardTimeout()
    {
        _logger.LogInformation("Simulating TimeoutException");
        
        throw new TimeoutException("The operation timed out after 30 seconds");
    }

    /// <summary>
    /// Simulates a standard NullReferenceException.
    /// Example: Unexpected null reference (coding error).
    /// </summary>
    [HttpGet("standard/null-reference")]
    public IActionResult ThrowNullReference()
    {
        _logger.LogInformation("Simulating NullReferenceException");
        
        string? nullString = null;
        return Ok(nullString!.Length); // This will throw NullReferenceException
    }

    /// <summary>
    /// Simulates a standard DivideByZeroException.
    /// Example: Mathematical error.
    /// </summary>
    [HttpGet("standard/divide-by-zero")]
    public IActionResult ThrowDivideByZero()
    {
        _logger.LogInformation("Simulating DivideByZeroException");
        
        int divisor = 0;
        int result = 100 / divisor; // This will throw DivideByZeroException
        return Ok(result);
    }

    #endregion

    #region Complex Scenarios

    /// <summary>
    /// Simulates a nested exception scenario.
    /// Example: Multiple layers of exception wrapping.
    /// </summary>
    [HttpGet("complex/nested")]
    public IActionResult ThrowNestedException()
    {
        _logger.LogInformation("Simulating nested exception");
        
        try
        {
            try
            {
                throw new InvalidOperationException("Inner database error");
            }
            catch (Exception innerEx)
            {
                throw new GenericDomainException(
                    ErrorType.Internal,
                    "DATABASE_ERROR",
                    "Failed to execute database operation",
                    innerEx);
            }
        }
        catch (Exception ex)
        {
            throw new GenericDomainException(
                ErrorType.Internal,
                "BUSINESS_LOGIC_ERROR",
                "Business logic failed during customer creation",
                ex);
        }
    }

    /// <summary>
    /// Simulates an AggregateException with multiple errors.
    /// Example: Parallel operations with multiple failures.
    /// </summary>
    [HttpGet("complex/aggregate")]
    public async Task<IActionResult> ThrowAggregateException()
    {
        _logger.LogInformation("Simulating aggregate exception");
        
        var tasks = new List<Task>
        {
            Task.Run(() => throw new InvalidOperationException("Service 1 failed")),
            Task.Run(() => throw new TimeoutException("Service 2 timed out")),
            Task.Run(() => throw new ArgumentException("Service 3 received invalid input"))
        };

        await Task.WhenAll(tasks); // This will throw AggregateException
        return Ok();
    }

    /// <summary>
    /// Simulates multiple validation errors.
    /// Example: Form with several invalid fields.
    /// </summary>
    [HttpPost("complex/multiple-validation")]
    public IActionResult ThrowMultipleValidation()
    {
        _logger.LogInformation("Simulating multiple validation errors");
        
        var errors = new Dictionary<string, string[]>
        {
            ["email"] = new[] { "Email is required", "Email format is invalid" },
            ["password"] = new[] { "Password must be at least 8 characters", "Password must contain a number" },
            ["age"] = new[] { "Age must be between 18 and 100" }
        };
        
        throw new ValidationException(errors);
    }

    /// <summary>
    /// Simulates multiple validation errors using Result pattern with DomainErrors.
    /// Example: Form with several invalid fields - Result pattern approach.
    /// </summary>
    [HttpPost("complex/multiple-validation-result")]
    public IActionResult MultipleValidationResult()
    {
        _logger.LogInformation("Simulating multiple validation errors using Result pattern");
        
        var errors = new List<DomainError>
        {
            DomainError.Validation("EMAIL_REQUIRED", "Email is required", "email"),
            DomainError.Validation("EMAIL_INVALID_FORMAT", "Email format is invalid", "email"),
            DomainError.Validation("PASSWORD_TOO_SHORT", "Password must be at least 8 characters", "password"),
            DomainError.Validation("PASSWORD_MISSING_NUMBER", "Password must contain a number", "password"),
            DomainError.Validation("AGE_OUT_OF_RANGE", "Age must be between 18 and 100", "age")
        };
        
        var domainErrors = DomainErrors.Multiple(errors);
        var result = Result<object, DomainErrors>.Failure(domainErrors);
        
        return result.ToActionResult();
    }

    /// <summary>
    /// Simulates a business rule violation with detailed context.
    /// Example: Complex business validation failure.
    /// </summary>
    [HttpPost("complex/business-rule")]
    public IActionResult ThrowBusinessRuleViolation()
    {
        _logger.LogInformation("Simulating business rule violation");
        
        throw new GenericDomainException(
            ErrorType.Validation,
            "BUSINESS_RULE_VIOLATION",
            "Cannot create order: Customer credit limit exceeded and payment method requires preauthorization");
    }

    /// <summary>
    /// Simulates a business rule violation using Result pattern with detailed context.
    /// </summary>
    [HttpPost("complex/business-rule-result")]
    public IActionResult BusinessRuleViolationResult()
    {
        _logger.LogInformation("Simulating business rule violation using Result pattern");
        
        var error = DomainError.Validation(
            "BUSINESS_RULE_VIOLATION",
            "Cannot create order: Customer credit limit exceeded and payment method requires preauthorization",
            "order",
            new Dictionary<string, object>
            {
                ["customerId"] = 12345,
                ["creditLimit"] = 5000.00m,
                ["orderTotal"] = 7500.00m,
                ["paymentMethod"] = "CreditCard",
                ["requiresPreauth"] = true
            });
        
        var result = Result<object, DomainError>.Failure(error);
        return result.ToActionResult();
    }

    #endregion

    #region Success Cases for Comparison

    /// <summary>
    /// Successful request for comparison.
    /// </summary>
    [HttpGet("success")]
    public IActionResult Success()
    {
        _logger.LogInformation("Successful request");
        
        return Ok(new
        {
            success = true,
            message = "Request processed successfully",
            data = new { id = 123, name = "Test Customer", status = "Active" },
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Successful request with Result pattern.
    /// </summary>
    [HttpGet("success-result")]
    public IActionResult SuccessResult()
    {
        _logger.LogInformation("Successful request using Result pattern");
        
        var customer = new { id = 123, name = "Test Customer", status = "Active" };
        var result = Result<object, DomainError>.Success(customer);
        
        return result.ToActionResult();
    }

    /// <summary>
    /// Successful request with Result pattern and custom message.
    /// </summary>
    [HttpGet("success-result-message")]
    public IActionResult SuccessResultWithMessage()
    {
        _logger.LogInformation("Successful request using Result pattern with custom message");
        
        var customer = new { id = 123, name = "Test Customer", status = "Active" };
        var result = Result<object, DomainError>.Success(customer);
        
        return result.ToActionResult("Customer retrieved successfully");
    }

    /// <summary>
    /// Async successful request with Result pattern.
    /// </summary>
    [HttpGet("success-result-async")]
    public async Task<IActionResult> SuccessResultAsync()
    {
        _logger.LogInformation("Async successful request using Result pattern");
        
        // Simulate async operation
        await Task.Delay(10);
        
        var customer = new { id = 456, name = "Async Customer", status = "Active" };
        var result = Result<object, DomainError>.Success(customer);
        
        return await Task.FromResult(result).ToActionResultAsync();
    }

    /// <summary>
    /// Demonstrates CRUD-specific GET result extension.
    /// Returns 200 OK with data or 204 NoContent if null.
    /// </summary>
    [HttpGet("crud/get/{id}")]
    public IActionResult CrudGetExample(int id)
    {
        _logger.LogInformation("CRUD GET example for ID: {Id}", id);
        
        // Simulate finding customer (null for ID > 100)
        var customer = id <= 100
            ? new { id, name = $"Customer {id}", status = "Active" }
            : null;
        
        var result = customer != null
            ? Result<object?, DomainError>.Success(customer)
            : Result<object?, DomainError>.Success(null);
        
        return result.ToGetActionResult();
    }

    /// <summary>
    /// Demonstrates CRUD-specific POST (Created) result extension.
    /// Returns 201 Created with Location header.
    /// </summary>
    [HttpPost("crud/create")]
    public IActionResult CrudCreateExample()
    {
        _logger.LogInformation("CRUD POST (Create) example");
        
        var newCustomer = new { id = 999, name = "New Customer", status = "Active" };
        var result = Result<object, DomainError>.Success(newCustomer);
        
        var locationUri = $"/api/customers/{newCustomer.id}";
        return result.ToCreatedActionResult(locationUri);
    }

    /// <summary>
    /// Demonstrates CRUD-specific PUT result extension.
    /// Returns 200 OK with data or 204 NoContent.
    /// </summary>
    [HttpPut("crud/update/{id}")]
    public IActionResult CrudUpdateExample(int id)
    {
        _logger.LogInformation("CRUD PUT (Update) example for ID: {Id}", id);
        
        var updatedCustomer = new { id, name = $"Updated Customer {id}", status = "Active" };
        var result = Result<object, DomainError>.Success(updatedCustomer);
        
        return result.ToPutActionResult();
    }

    /// <summary>
    /// Demonstrates CRUD-specific DELETE result extension.
    /// Returns 204 NoContent on success or 404 NotFound.
    /// </summary>
    [HttpDelete("crud/delete/{id}")]
    public IActionResult CrudDeleteExample(int id)
    {
        _logger.LogInformation("CRUD DELETE example for ID: {Id}", id);
        
        // Simulate deletion (fails for ID > 100)
        bool deleted = id <= 100;
        
        var result = deleted
            ? Result<bool, DomainError>.Success(true)
            : Result<bool, DomainError>.Failure(
                DomainError.NotFound("CUSTOMER_NOT_FOUND", $"Customer {id} not found"));
        
        return result.ToDeleteActionResult();
    }

    #endregion

    #region Random Exception Generator

    /// <summary>
    /// Randomly throws one of the various exception types.
    /// Useful for testing exception handling in unpredictable scenarios.
    /// </summary>
    [HttpGet("random")]
    public IActionResult ThrowRandomException()
    {
        var random = Random.Shared.Next(1, 16);
        
        _logger.LogInformation("Throwing random exception type: {Type}", random);
        
        return random switch
        {
            1 => ThrowValidationError(),
            2 => ThrowNotFound(123),
            3 => ThrowConflict(),
            4 => ThrowUnauthorized(),
            5 => ThrowForbidden(),
            6 => ThrowInternalError(),
            7 => ThrowServiceUnavailable(),
            8 => ThrowTimeout(),
            9 => ThrowSqlForeignKeyViolation(),
            10 => ThrowSqlUniqueViolation(),
            11 => ThrowArgumentNull(),
            12 => ThrowInvalidOperation(),
            13 => ThrowStandardTimeout(),
            14 => ThrowRateLimited(),
            15 => ThrowPayloadTooLarge(),
            _ => Success()
        };
    }

    /// <summary>
    /// Randomly returns either success or failure using Result pattern.
    /// </summary>
    [HttpGet("random-result")]
    public IActionResult RandomResult()
    {
        var random = Random.Shared.Next(1, 11);
        
        _logger.LogInformation("Random Result pattern response type: {Type}", random);
        
        return random switch
        {
            1 => ValidationErrorResult(),
            2 => NotFoundResult(123),
            3 => ConflictResult(),
            4 => UnauthorizedResult(),
            5 => ForbiddenResult(),
            6 => InternalErrorResult(),
            7 => ServiceUnavailableResult(),
            8 => TimeoutResult(),
            9 => ExternalErrorResult(),
            _ => SuccessResult()
        };
    }

    #endregion

    #region Documentation Endpoint

    /// <summary>
    /// Returns documentation about all available exception test endpoints.
    /// </summary>
    [HttpGet("info")]
    public IActionResult GetTestInfo()
    {
        return Ok(new
        {
            description = "Exception Testing Controller",
            purpose = "Simulate various exception scenarios to test ApiExceptionMiddleware and demonstrate Result pattern",
            approaches = new
            {
                exceptionThrowing = "Endpoints that throw exceptions - middleware catches and handles",
                resultPattern = "Endpoints suffixed with '-result' that use Result<T, DomainError> pattern"
            },
            categories = new
            {
                clientErrors = new[]
                {
                    new { endpoint = "POST /validation-error", status = 422, code = "INVALID_EMAIL", approach = "Exception" },
                    new { endpoint = "POST /validation-error-result", status = 422, code = "INVALID_EMAIL", approach = "Result" },
                    new { endpoint = "POST /bad-request", status = 400, code = "INVALID_JSON", approach = "Exception" },
                    new { endpoint = "POST /bad-request-result", status = 400, code = "INVALID_JSON", approach = "Result" },
                    new { endpoint = "GET /not-found/{id}", status = 404, code = "CUSTOMER_NOT_FOUND", approach = "Exception" },
                    new { endpoint = "GET /not-found-result/{id}", status = 404, code = "CUSTOMER_NOT_FOUND", approach = "Result" },
                    new { endpoint = "POST /conflict", status = 409, code = "DUPLICATE_EMAIL", approach = "Exception" },
                    new { endpoint = "POST /conflict-result", status = 409, code = "DUPLICATE_EMAIL", approach = "Result" },
                    new { endpoint = "GET /unauthorized", status = 401, code = "INVALID_TOKEN", approach = "Exception" },
                    new { endpoint = "GET /unauthorized-result", status = 401, code = "INVALID_TOKEN", approach = "Result" },
                    new { endpoint = "GET /forbidden", status = 403, code = "INSUFFICIENT_PERMISSIONS", approach = "Exception" },
                    new { endpoint = "GET /forbidden-result", status = 403, code = "INSUFFICIENT_PERMISSIONS", approach = "Result" },
                    new { endpoint = "GET /rate-limited", status = 429, code = "RATE_LIMIT_EXCEEDED", approach = "Exception" },
                    new { endpoint = "GET /rate-limited-result", status = 429, code = "RATE_LIMIT_EXCEEDED", approach = "Result" },
                    new { endpoint = "POST /payload-too-large", status = 413, code = "PAYLOAD_TOO_LARGE", approach = "Exception" }
                },
                serverErrors = new[]
                {
                    new { endpoint = "GET /internal-error", status = 500, code = "INTERNAL_ERROR", approach = "Exception" },
                    new { endpoint = "GET /internal-error-result", status = 500, code = "INTERNAL_ERROR", approach = "Result" },
                    new { endpoint = "GET /service-unavailable", status = 503, code = "SERVICE_UNAVAILABLE", approach = "Exception" },
                    new { endpoint = "GET /service-unavailable-result", status = 503, code = "SERVICE_UNAVAILABLE", approach = "Result" },
                    new { endpoint = "GET /timeout", status = 504, code = "EXTERNAL_SERVICE_TIMEOUT", approach = "Exception" },
                    new { endpoint = "GET /timeout-result", status = 504, code = "EXTERNAL_SERVICE_TIMEOUT", approach = "Result" },
                    new { endpoint = "GET /not-implemented", status = 501, code = "FEATURE_NOT_IMPLEMENTED", approach = "Exception" },
                    new { endpoint = "GET /external-error", status = 502, code = "EXTERNAL_API_ERROR", approach = "Exception" },
                    new { endpoint = "GET /external-error-result", status = 502, code = "EXTERNAL_API_ERROR", approach = "Result" }
                },
                sqlErrors = new[]
                {
                    new { endpoint = "DELETE /sql/foreign-key-violation", status = 409, code = "FK_VIOLATION" },
                    new { endpoint = "POST /sql/unique-violation", status = 409, code = "UNIQUE_VIOLATION" },
                    new { endpoint = "GET /sql/timeout", status = 504, code = "SQL_TIMEOUT" },
                    new { endpoint = "POST /sql/deadlock", status = 409, code = "SQL_DEADLOCK" },
                    new { endpoint = "GET /sql/connection-error", status = 503, code = "SQL_CONNECTION_FAILED" }
                },
                standardExceptions = new[]
                {
                    new { endpoint = "POST /standard/argument-null", status = 500, description = "ArgumentNullException" },
                    new { endpoint = "POST /standard/argument-invalid", status = 500, description = "ArgumentException" },
                    new { endpoint = "POST /standard/invalid-operation", status = 500, description = "InvalidOperationException" },
                    new { endpoint = "GET /standard/timeout", status = 500, description = "TimeoutException" },
                    new { endpoint = "GET /standard/null-reference", status = 500, description = "NullReferenceException" },
                    new { endpoint = "GET /standard/divide-by-zero", status = 500, description = "DivideByZeroException" }
                },
                complex = new[]
                {
                    new { endpoint = "GET /complex/nested", description = "Nested exceptions" },
                    new { endpoint = "GET /complex/aggregate", description = "AggregateException with multiple errors" },
                    new { endpoint = "POST /complex/multiple-validation", description = "Multiple validation errors (Exception)" },
                    new { endpoint = "POST /complex/multiple-validation-result", description = "Multiple validation errors (Result)" },
                    new { endpoint = "POST /complex/business-rule", description = "Business rule violation (Exception)" },
                    new { endpoint = "POST /complex/business-rule-result", description = "Business rule violation (Result)" }
                },
                crudPatterns = new[]
                {
                    new { endpoint = "GET /crud/get/{id}", description = "GET with 200 OK or 204 NoContent" },
                    new { endpoint = "POST /crud/create", description = "POST with 201 Created" },
                    new { endpoint = "PUT /crud/update/{id}", description = "PUT with 200 OK or 204 NoContent" },
                    new { endpoint = "DELETE /crud/delete/{id}", description = "DELETE with 204 NoContent or 404 NotFound" }
                },
                success = new[]
                {
                    new { endpoint = "GET /success", description = "Standard success response" },
                    new { endpoint = "GET /success-result", description = "Success with Result pattern" },
                    new { endpoint = "GET /success-result-message", description = "Success with custom message" },
                    new { endpoint = "GET /success-result-async", description = "Async success with Result pattern" }
                },
                utility = new[]
                {
                    new { endpoint = "GET /random", description = "Random exception type" },
                    new { endpoint = "GET /random-result", description = "Random Result pattern response" },
                    new { endpoint = "GET /info", description = "This endpoint - shows all available tests" }
                }
            },
            usage = new
            {
                baseUrl = "/api/ExceptionTest",
                exampleComparisons = new[]
                {
                    new 
                    { 
                        scenario = "Validation Error",
                        exceptionBased = "POST /api/ExceptionTest/validation-error",
                        resultBased = "POST /api/ExceptionTest/validation-error-result",
                        note = "Both produce identical API responses"
                    },
                    new 
                    { 
                        scenario = "Not Found",
                        exceptionBased = "GET /api/ExceptionTest/not-found/123",
                        resultBased = "GET /api/ExceptionTest/not-found-result/123",
                        note = "Result pattern avoids exception overhead"
                    },
                    new 
                    { 
                        scenario = "Multiple Errors",
                        exceptionBased = "POST /api/ExceptionTest/complex/multiple-validation",
                        resultBased = "POST /api/ExceptionTest/complex/multiple-validation-result",
                        note = "Result pattern with DomainErrors for multiple issues"
                    }
                }
            },
            extensionMethods = new
            {
                basic = new[]
                {
                    "ToActionResult() - Convert Result to IActionResult",
                    "ToActionResult(message) - With custom success message",
                    "ToActionResultAsync() - Async version"
                },
                crud = new[]
                {
                    "ToGetActionResult() - 200 OK or 204 NoContent",
                    "ToCreatedActionResult(uri) - 201 Created with Location",
                    "ToPutActionResult() - 200 OK or 204 NoContent",
                    "ToDeleteActionResult() - 204 NoContent or 404 NotFound"
                },
                errors = new[]
                {
                    "Result<T, DomainError> - Single error",
                    "Result<T, DomainErrors> - Multiple errors",
                    "DomainError.Validation/NotFound/Conflict/etc. - Factory methods"
                }
            }
        });
    }

    #endregion
}

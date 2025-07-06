using Acontplus.Core.Domain.Enums;

namespace Acontplus.Persistence.SqlServer.Exceptions;

public static class SqlServerExceptionHandler
{
    /// <summary>
    /// Enhanced transient error detection for SQL Server 2022+ and Azure SQL
    /// </summary>
    public static bool IsTransientException(SqlException ex)
    {
        // Modern SQL Server 2022+ and Azure SQL transient errors
        var transientErrorNumbers = new HashSet<int>
        {
            // Connection and timeout errors
            2,      // Timeout
            53,     // Network path not found
            64,     // Connection failed
            233,    // Connection failed
            10053,  // Connection aborted
            10054,  // Connection reset
            10060,  // Connection timeout
            11001,  // Host not found
            
            // Database availability errors
            4060,   // Database unavailable
            40197,  // Service busy
            40501,  // Service busy
            40613,  // Database unavailable
            
            // Azure SQL specific errors
            40143,  // Database has reached its size quota
            40149,  // Database unavailable
            40197,  // Service has too many requests
            40501,  // Service is currently busy
            40544,  // Database has reached its DTU quota
            40549,  // Session terminated (long-running transaction)
            40613,  // Database is currently unavailable
            
            // Processing errors
            49918,  // Cannot process request
            49919,  // Cannot process request
            49920,  // Cannot process request
            
            // Memory and resource errors
            8645,   // Timeout waiting for memory resource
            8651,   // Could not perform the requested operation
            
            // Deadlock (retryable)
            1205,   // Deadlock victim
            
            // Transient authentication issues (connection pooling)
            18456 when ex.Class == 14, // Login failed due to connection pooling
        };
        
        return transientErrorNumbers.Contains(ex.Number);
    }

    /// <summary>
    /// Maps SQL Server exceptions to domain error types with enhanced error codes
    /// </summary>
    public static SqlErrorInfo MapSqlException(SqlException ex)
    {
        return ex.Number switch
        {
            // Timeout errors
            2 => new SqlErrorInfo(ErrorType.Timeout, "SQL_TIMEOUT", 
                "Database operation timed out", ex),
            
            // Service unavailable errors
            4060 or 40197 or 40501 or 40613 or 40143 or 40149 or 40544 or 40549 
                => new SqlErrorInfo(ErrorType.ServiceUnavailable, "SQL_SERVICE_UNAVAILABLE", 
                    "Database service is currently unavailable", ex),
            
            // Authentication/Authorization errors
            18456 => new SqlErrorInfo(ErrorType.Unauthorized, "SQL_AUTH_FAILED", 
                "Database authentication failed", ex),
            18461 => new SqlErrorInfo(ErrorType.Unauthorized, "SQL_LOGIN_FAILED", 
                "Login failed for user", ex),
            
            // Connection errors (Infrastructure)
            53 or 64 or 233 or 10053 or 10054 or 10060 or 11001 
                => new SqlErrorInfo(ErrorType.External, "SQL_CONNECTION_FAILED", 
                    "Database connection failed", ex),
            
            // Constraint violations (Conflict)
            2627 => new SqlErrorInfo(ErrorType.Conflict, "SQL_DUPLICATE_KEY", 
                "Duplicate key violation", ex),
            2601 => new SqlErrorInfo(ErrorType.Conflict, "SQL_DUPLICATE_INDEX", 
                "Duplicate index violation", ex),
            547 => new SqlErrorInfo(ErrorType.Conflict, "SQL_FOREIGN_KEY_VIOLATION", 
                "Foreign key constraint violation", ex),
            
            // Validation errors
            515 => new SqlErrorInfo(ErrorType.Validation, "SQL_REQUIRED_FIELD_NULL", 
                "Required field cannot be null", ex),
            547 when ex.Message.Contains("CHECK constraint") 
                => new SqlErrorInfo(ErrorType.Validation, "SQL_CHECK_CONSTRAINT", 
                    "Check constraint violation", ex),
            8152 => new SqlErrorInfo(ErrorType.Validation, "SQL_STRING_TOO_LONG", 
                "String or binary data would be truncated", ex),
            245 => new SqlErrorInfo(ErrorType.Validation, "SQL_CONVERSION_ERROR", 
                "Conversion failed when converting value", ex),
            
            // Deadlock (Conflict - retryable)
            1205 => new SqlErrorInfo(ErrorType.Conflict, "SQL_DEADLOCK", 
                "Transaction was deadlocked", ex),
            
            // Custom application errors from stored procedures
            >= 50000 and <= 99999 => new SqlErrorInfo(ErrorType.Validation, $"SP_ERROR_{ex.Number}", 
                ex.Message, ex),
            
            // Business logic errors (severity 16 from stored procedures)
            _ when ex is { Class: 16, Number: >= 50000 } 
                => new SqlErrorInfo(ErrorType.Validation, $"SP_BUSINESS_ERROR_{ex.Number}", 
                    ex.Message, ex),
            
            // Permission errors
            229 => new SqlErrorInfo(ErrorType.Forbidden, "SQL_PERMISSION_DENIED", 
                "Permission denied", ex),
            
            // Resource errors
            8645 or 8651 => new SqlErrorInfo(ErrorType.ServiceUnavailable, "SQL_RESOURCE_UNAVAILABLE", 
                "Database resources temporarily unavailable", ex),
            
            // All other errors as internal
            _ => new SqlErrorInfo(ErrorType.Internal, $"SQL_ERROR_{ex.Number}", 
                ex.Message, ex)
        };
    }

    /// <summary>
    /// Handles SQL exceptions with enhanced logging and domain error mapping
    /// </summary>
    public static void HandleSqlException(SqlException ex, ILogger logger, string operation)
    {
        var errorInfo = MapSqlException(ex);
        
        // Enhanced logging based on error type
        var logLevel = errorInfo.Type switch
        {
            ErrorType.Validation or ErrorType.Conflict or ErrorType.NotFound 
                => LogLevel.Warning,
            ErrorType.Unauthorized or ErrorType.Forbidden 
                => LogLevel.Warning,
            ErrorType.Timeout or ErrorType.ServiceUnavailable 
                => LogLevel.Warning,
            ErrorType.External or ErrorType.Internal 
                => LogLevel.Error,
            _ => LogLevel.Error
        };

        logger.Log(logLevel, ex, 
            "SQL Error in {Operation}: Type={ErrorType}, Code={ErrorCode}, SqlNumber={SqlNumber}, Message={Message}", 
            operation, errorInfo.Type, errorInfo.Code, ex.Number, ex.Message);

        // Include additional context for debugging
        if (logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug("SQL Error Details: Severity={Severity}, State={State}, Procedure={Procedure}, LineNumber={LineNumber}",
                ex.Class, ex.State, ex.Procedure, ex.LineNumber);
        }

        throw new SqlDomainException(errorInfo.Type, errorInfo.Code, errorInfo.Message, ex);
    }

    /// <summary>
    /// Maps SQL error types to HTTP status codes for API responses
    /// </summary>
    public static int MapErrorTypeToHttpStatus(ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.Validation => 422,        // Unprocessable Entity
            ErrorType.NotFound => 404,          // Not Found
            ErrorType.Conflict => 409,          // Conflict
            ErrorType.Unauthorized => 401,      // Unauthorized
            ErrorType.Forbidden => 403,         // Forbidden
            ErrorType.Timeout => 408,           // Request Timeout
            ErrorType.ServiceUnavailable => 503, // Service Unavailable
            ErrorType.RateLimited => 429,       // Too Many Requests
            ErrorType.External => 502,          // Bad Gateway
            ErrorType.Internal => 500,          // Internal Server Error
            _ => 500                            // Default to Internal Server Error
        };
    }
}
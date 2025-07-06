using Acontplus.Core.Domain.Enums;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Acontplus.Persistence.SqlServer.Exceptions;

public static class SqlServerExceptionHandler
{
    // Immutable collections for thread-safe error mappings
    private static readonly ImmutableHashSet<int> TransientErrorNumbers = ImmutableHashSet.Create(
        // Connection and timeout errors
        2, // Timeout
        53, // Network path not found
        64, // Connection failed
        233, // Connection failed
        10053, // Connection aborted
        10054, // Connection reset
        10060, // Connection timeout
        11001, // Host not found

        // Database availability errors
        4060, // Database unavailable
        40197, // Service busy
        40501, // Service busy
        40613, // Database unavailable

        // Azure SQL specific errors
        40143, // Database has reached its size quota
        40149, // Database unavailable
        40544, // Database has reached its DTU quota
        40549, // Session terminated (long-running transaction)

        // Processing errors
        49918, // Cannot process request
        49919, // Cannot process request
        49920, // Cannot process request

        // Memory and resource errors
        8645, // Timeout waiting for memory resource
        8651, // Could not perform the requested operation

        // Deadlock (retryable)
        1205 // Deadlock victim
    );

    private static readonly ImmutableDictionary<int, SqlErrorInfo> ErrorMappings =
        new Dictionary<int, SqlErrorInfo>
        {
            // Timeout errors
            [2] = new(ErrorType.Timeout, "SQL_TIMEOUT", "Database operation timed out"),

            // Service unavailable errors
            [4060] = new(ErrorType.ServiceUnavailable, "SQL_SERVICE_UNAVAILABLE",
                "Database service is currently unavailable"),
            [40197] = new(ErrorType.ServiceUnavailable, "SQL_SERVICE_BUSY", "Service is busy"),
            [40613] =
                new(ErrorType.ServiceUnavailable, "SQL_DATABASE_UNAVAILABLE", "Database is currently unavailable"),

            // Authentication/Authorization errors
            [18456] = new(ErrorType.Unauthorized, "SQL_AUTH_FAILED", "Database authentication failed"),
            [18461] = new(ErrorType.Unauthorized, "SQL_LOGIN_FAILED", "Login failed for user"),

            // Connection errors (Infrastructure)
            [53] = new(ErrorType.External, "SQL_CONNECTION_FAILED", "Network path not found"),
            [64] = new(ErrorType.External, "SQL_CONNECTION_FAILED", "Network connection failed"),

            // Constraint violations (Conflict)
            [2627] = new(ErrorType.Conflict, "SQL_DUPLICATE_KEY", "Duplicate key violation"),
            [2601] = new(ErrorType.Conflict, "SQL_DUPLICATE_INDEX", "Duplicate index violation"),
            [547] = new(ErrorType.Conflict, "SQL_FOREIGN_KEY_VIOLATION", "Foreign key constraint violation"),

            // Validation errors
            [515] = new(ErrorType.Validation, "SQL_REQUIRED_FIELD_NULL", "Required field cannot be null"),
            [8152] = new(ErrorType.Validation, "SQL_STRING_TOO_LONG", "String or binary data would be truncated"),
            [245] = new(ErrorType.Validation, "SQL_CONVERSION_ERROR", "Conversion failed when converting value"),

            // Deadlock (Conflict - retryable)
            [1205] = new(ErrorType.Conflict, "SQL_DEADLOCK", "Transaction was deadlocked"),

            // Permission errors
            [229] = new(ErrorType.Forbidden, "SQL_PERMISSION_DENIED", "Permission denied"),

            // Resource errors
            [8645] = new(ErrorType.ServiceUnavailable, "SQL_RESOURCE_UNAVAILABLE",
                "Memory resources temporarily unavailable"),
        }.ToImmutableDictionary();

    /// <summary>
    /// Determines if a SQL exception is transient and might succeed if retried
    /// </summary>
    public static bool IsTransientException(SqlException ex)
    {
        // Special case for authentication errors that might be connection pooling issues
        if (ex.Number == 18456 && ex.Class == 14)
        {
            return true;
        }

        return TransientErrorNumbers.Contains(ex.Number);
    }

    /// <summary>
    /// Maps SQL Server exceptions to domain error types with enhanced error codes
    /// </summary>
    public static SqlErrorInfo MapSqlException(SqlException ex)
    {
        if (ErrorMappings.TryGetValue(ex.Number, out var errorInfo))
        {
            return errorInfo with { Exception = ex };
        }

        // Handle special cases that require message inspection
        if (ex.Number == 547 && ex.Message.Contains("CHECK constraint"))
        {
            return new SqlErrorInfo(
                ErrorType.Validation,
                "SQL_CHECK_CONSTRAINT",
                "Check constraint violation",
                ex);
        }

        // Custom application errors from stored procedures
        if (ex.Number is >= 50000 and <= 99999)
        {
            return ex.Class == 16
                ? new SqlErrorInfo(ErrorType.Validation, $"SP_BUSINESS_ERROR_{ex.Number}", ex.Message, ex)
                : new SqlErrorInfo(ErrorType.Validation, $"SP_ERROR_{ex.Number}", ex.Message, ex);
        }

        // Default case for unmapped errors
        return new SqlErrorInfo(
            ErrorType.Internal,
            $"SQL_ERROR_{ex.Number}",
            ex.Message,
            ex);
    }

    /// <summary>
    /// Handles SQL exceptions with structured logging and domain error mapping
    /// </summary>
    public static void HandleSqlException(
        SqlException ex,
        ILogger logger,
        string operation,
        [CallerMemberName] string caller = "")
    {
        var errorInfo = MapSqlException(ex);

        // Structured logging with log level determination
        var logLevel = errorInfo.Type switch
        {
            ErrorType.Validation or ErrorType.Conflict or ErrorType.NotFound => LogLevel.Warning,
            ErrorType.Unauthorized or ErrorType.Forbidden => LogLevel.Warning,
            ErrorType.Timeout or ErrorType.ServiceUnavailable => LogLevel.Warning,
            _ => LogLevel.Error
        };

        logger.Log(logLevel,
            new EventId(ex.Number, errorInfo.Code),
            "SQL Error in {Operation} called from {Caller}: {ErrorType} - {Message}",
            operation,
            caller,
            errorInfo.Type,
            errorInfo.Message);

        // Detailed debug information when enabled
        if (logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug("SQL Error Details: {Details}", new
            {
                ex.Number,
                ex.Class,
                ex.State,
                ex.Procedure,
                ex.LineNumber,
                ex.Server,
                ex.ClientConnectionId
            });
        }

        throw new SqlDomainException(errorInfo);
    }
}
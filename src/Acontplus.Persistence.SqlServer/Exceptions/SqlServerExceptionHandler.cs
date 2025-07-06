using Acontplus.Core.Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Acontplus.Persistence.SqlServer.Exceptions;

public static class SqlServerExceptionHandler
{
    private static class ErrorRanges
    {
        // SQL Server 2022+ valid custom error range (13000-2147483647, excluding 50000)
        public const int MinCustomError = 13000;
        public const int MaxCustomError = int.MaxValue;
        public const int ReservedError = 50000;

        // Standard transient errors (connection, timeouts, etc.)
        public static readonly ImmutableHashSet<int> TransientErrors = ImmutableHashSet.Create(
            // Connection and timeout errors
            2, 53, 64, 233, // Basic connection issues
            10053, 10054, 10060, // Connection aborted/reset/timeout
            11001, // Host not found

            // Database availability
            4060, 40197, 40501, 40613,

            // Azure SQL specific
            40143, 40149, 40544, 40549,

            // Processing errors
            49918, 49919, 49920,

            // Resource issues
            8645, 8651,

            // Deadlock
            1205
        );

        // Error categories for better classification
        public static class CustomErrors
        {
            public const int ValidationStart = 13000;
            public const int BusinessRuleStart = 14000;
            public const int DataAccessStart = 15000;
            public const int SecurityStart = 16000;
        }
    }

    private static readonly ImmutableDictionary<int, SqlErrorInfo> ErrorMappings = new Dictionary<int, SqlErrorInfo>
    {
        // Timeout errors
        [2] = new(ErrorType.Timeout, "SQL_TIMEOUT", "Database operation timed out"),

        // Service unavailable
        [4060] = new(ErrorType.ServiceUnavailable, "SQL_SERVICE_UNAVAILABLE",
            "Database service is currently unavailable"),
        [40197] = new(ErrorType.ServiceUnavailable, "SQL_SERVICE_BUSY", "Service is busy"),
        [40613] = new(ErrorType.ServiceUnavailable, "SQL_DATABASE_UNAVAILABLE",
            "Database is currently unavailable"),

        // Authentication
        [18456] = new(ErrorType.Unauthorized, "SQL_AUTH_FAILED", "Database authentication failed"),
        [18461] = new(ErrorType.Unauthorized, "SQL_LOGIN_FAILED", "Login failed for user"),

        // Connection
        [53] = new(ErrorType.External, "SQL_CONNECTION_FAILED", "Network path not found"),
        [64] = new(ErrorType.External, "SQL_CONNECTION_FAILED", "Network connection failed"),

        // Constraints
        [2627] = new(ErrorType.Conflict, "SQL_DUPLICATE_KEY", "Duplicate key violation"),
        [2601] = new(ErrorType.Conflict, "SQL_DUPLICATE_INDEX", "Duplicate index violation"),
        [547] = new(ErrorType.Conflict, "SQL_FOREIGN_KEY_VIOLATION", "Foreign key constraint violation"),

        // Validation
        [515] = new(ErrorType.Validation, "SQL_REQUIRED_FIELD_NULL", "Required field cannot be null"),
        [8152] = new(ErrorType.Validation, "SQL_STRING_TOO_LONG", "String or binary data would be truncated"),
        [245] = new(ErrorType.Validation, "SQL_CONVERSION_ERROR", "Conversion failed when converting value"),

        // Deadlock
        [1205] = new(ErrorType.Conflict, "SQL_DEADLOCK", "Transaction was deadlocked"),

        // Permission
        [229] = new(ErrorType.Forbidden, "SQL_PERMISSION_DENIED", "Permission denied"),

        // Resource
        [8645] = new(ErrorType.ServiceUnavailable, "SQL_RESOURCE_UNAVAILABLE",
            "Memory resources temporarily unavailable"),
    }.ToImmutableDictionary();

    public static bool IsTransientException(SqlException ex)
    {
        // Special case for transient authentication errors
        if (ex.Number == 18456 && ex.Class == 14)
        {
            return true;
        }

        return ErrorRanges.TransientErrors.Contains(ex.Number);
    }

    public static SqlErrorInfo MapSqlException(SqlException ex)
    {
        // Try to get mapped error first
        if (ErrorMappings.TryGetValue(ex.Number, out var errorInfo))
        {
            return errorInfo with { Exception = ex };
        }

        // Handle special constraint cases
        if (ex.Number == 547 && ex.Message.Contains("CHECK constraint"))
        {
            return new SqlErrorInfo(
                ErrorType.Validation,
                "SQL_CHECK_CONSTRAINT",
                "Check constraint violation",
                ex);
        }

        // Handle modern stored procedure errors (13000+)
        if (IsCustomStoredProcedureError(ex.Number))
        {
            return HandleCustomStoredProcedureError(ex);
        }

        // Default case
        return new SqlErrorInfo(
            ErrorType.Internal,
            $"SQL_ERROR_{ex.Number}",
            ex.Message,
            ex);
    }

    private static bool IsCustomStoredProcedureError(int errorNumber)
    {
        return errorNumber >= ErrorRanges.MinCustomError &&
               errorNumber <= ErrorRanges.MaxCustomError &&
               errorNumber != ErrorRanges.ReservedError;
    }

    private static SqlErrorInfo HandleCustomStoredProcedureError(SqlException ex)
    {
        var errorType = ex.Number switch
        {
            >= ErrorRanges.CustomErrors.ValidationStart and < ErrorRanges.CustomErrors.BusinessRuleStart
                => ErrorType.Validation,
            >= ErrorRanges.CustomErrors.SecurityStart
                => ErrorType.Forbidden,
            _ => ErrorType.Validation
        };

        var prefix = ex.Class == 16 ? "SP_BUSINESS_" : "SP_";
        return new SqlErrorInfo(
            errorType,
            $"{prefix}{ex.Number}",
            ex.Message,
            ex);
    }

    public static void HandleSqlException(
        SqlException ex,
        ILogger logger,
        string operation,
        [CallerMemberName] string caller = "")
    {
        var errorInfo = MapSqlException(ex);

        logger.Log(GetLogLevel(errorInfo.Type),
            new EventId(ex.Number, errorInfo.Code),
            "SQL Error in {Operation} called from {Caller}: {ErrorType} - {Message}",
            operation, caller, errorInfo.Type, errorInfo.Message);

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
                ex.ClientConnectionId,
                errorInfo.Code,
                IsTransient = IsTransientException(ex)
            });
        }

        throw new SqlDomainException(errorInfo);
    }

    private static LogLevel GetLogLevel(ErrorType errorType) => errorType switch
    {
        ErrorType.Validation or ErrorType.Conflict or ErrorType.NotFound => LogLevel.Warning,
        ErrorType.Unauthorized or ErrorType.Forbidden => LogLevel.Warning,
        ErrorType.Timeout or ErrorType.ServiceUnavailable => LogLevel.Warning,
        _ => LogLevel.Error
    };
}
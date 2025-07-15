namespace Acontplus.Utilities.Adapters;

/// <summary>
/// Provides helpers to map SQL Server error codes to domain errors.
/// </summary>
public static class SqlResponseAdapter
{
    /// <summary>
    /// Maps a SQL Server error code and message to a <see cref="DomainError"/>.
    /// </summary>
    /// <param name="errorCode">The SQL error code as a string.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A <see cref="DomainError"/> representing the error.</returns>
    public static DomainError MapSqlServerError(string errorCode, string message)
    {
        if (int.TryParse(errorCode, out int sqlErrorNumber))
        {
            return MapSqlErrorNumber(sqlErrorNumber, message);
        }
        return DomainError.Internal("SERVER_ERROR", message ?? "Unknown database error");
    }

    /// <summary>
    /// Maps a SQL Server error number to a <see cref="DomainError"/> with a user-friendly message.
    /// </summary>
    /// <param name="sqlErrorNumber">The SQL Server error number.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A <see cref="DomainError"/>.</returns>
    public static DomainError MapSqlErrorNumber(int sqlErrorNumber, string message)
    {
        return sqlErrorNumber switch
        {
            // Primary Key Violation
            2627 => DomainError.Conflict("DUPLICATE_KEY", "A record with this key already exists."),

            // Unique Key Violation
            2601 => DomainError.Conflict("DUPLICATE_VALUE", "A record with this value already exists."),

            // Foreign Key Violation
            547 => DomainError.Validation("FOREIGN_KEY_VIOLATION", "Referenced record does not exist or cannot be deleted due to dependencies."),

            // Null Constraint Violation
            515 => DomainError.Validation("NULL_CONSTRAINT_VIOLATION", "Required field cannot be null."),

            // Invalid Column Name
            207 => DomainError.Validation("INVALID_COLUMN", "Invalid column name specified."),

            // Invalid Object Name (Table/View not found)
            208 => DomainError.Internal("INVALID_OBJECT", "Database object not found."),

            // Permission Denied
            229 => DomainError.Forbidden("PERMISSION_DENIED", "Insufficient permissions to perform this operation."),

            // Login Failed
            18456 => DomainError.Unauthorized("LOGIN_FAILED", "Authentication failed."),
            18461 => DomainError.Unauthorized("LOGIN_FAILED", "Login failed for user."),

            // Timeout
            -2 => DomainError.Timeout("DATABASE_TIMEOUT", "Database operation timed out."),
            2 => DomainError.Timeout("SQL_TIMEOUT", "Database operation timed out."),
            53 => DomainError.ServiceUnavailable("SQL_CONNECTION_FAILED", "Network path not found."),
            64 => DomainError.ServiceUnavailable("SQL_CONNECTION_FAILED", "Network connection failed."),
            10053 => DomainError.ServiceUnavailable("SQL_CONNECTION_ABORTED", "Connection aborted."),
            10054 => DomainError.ServiceUnavailable("SQL_CONNECTION_RESET", "Connection reset by peer."),
            10060 => DomainError.ServiceUnavailable("SQL_CONNECTION_TIMEOUT", "Connection timed out."),
            11001 => DomainError.ServiceUnavailable("SQL_HOST_NOT_FOUND", "Host not found."),

            // Deadlock
            1205 => DomainError.Conflict("DEADLOCK_DETECTED", "Database deadlock detected. Please retry the operation."),

            // Lock Timeout
            1222 => DomainError.Timeout("LOCK_TIMEOUT", "Database lock timeout. Please retry the operation."),

            // Invalid Data Type
            245 => DomainError.Validation("INVALID_DATA_TYPE", "Invalid data type for the specified field."),

            // String or Binary Data Truncation
            8152 => DomainError.Validation("DATA_TRUNCATION", "Data is too long for the specified field."),

            // Arithmetic Overflow
            8115 => DomainError.Validation("ARITHMETIC_OVERFLOW", "Arithmetic overflow error."),

            // Division by Zero
            8134 => DomainError.Validation("DIVISION_BY_ZERO", "Division by zero error."),

            // Stored Procedure Not Found
            2812 => DomainError.Internal("PROCEDURE_NOT_FOUND", "Stored procedure not found."),

            // Invalid Parameter Count
            8144 => DomainError.Validation("INVALID_PARAMETER_COUNT", "Invalid number of parameters specified."),

            // Transaction Rollback
            3961 => DomainError.Internal("TRANSACTION_ROLLBACK", "Transaction was rolled back due to an error."),

            // Database Full
            1105 => DomainError.ServiceUnavailable("DATABASE_FULL", "Database storage is full."),

            // Log Full
            9002 => DomainError.ServiceUnavailable("LOG_FULL", "Database log is full."),

            // Service Unavailable
            4060 => DomainError.ServiceUnavailable("SQL_SERVICE_UNAVAILABLE", "Database service is currently unavailable."),
            40197 => DomainError.ServiceUnavailable("SQL_SERVICE_BUSY", "Service is busy."),
            40613 => DomainError.ServiceUnavailable("SQL_DATABASE_UNAVAILABLE", "Database is currently unavailable."),
            49918 => DomainError.ServiceUnavailable("SQL_RESOURCE_LIMIT", "Resource limit reached."),
            49919 => DomainError.ServiceUnavailable("SQL_RESOURCE_LIMIT", "Resource limit reached."),
            49920 => DomainError.ServiceUnavailable("SQL_RESOURCE_LIMIT", "Resource limit reached."),
            8645 => DomainError.ServiceUnavailable("SQL_RESOURCE_UNAVAILABLE", "Memory resources temporarily unavailable."),
            8651 => DomainError.ServiceUnavailable("SQL_RESOURCE_UNAVAILABLE", "Memory resources temporarily unavailable."),

            // Default case for unknown SQL errors
            _ => DomainError.Internal("SQL_ERROR", $"Database error occurred. Error code: {sqlErrorNumber}. Message: {message}")
        };
    }
}
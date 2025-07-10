namespace Acontplus.Utilities.Adapters;

/// <summary>
/// Provides extension methods to adapt SQL stored procedure responses to API results.
/// </summary>
public static class SqlResponseAdapter
{
    /// <summary>
    /// Converts a <see cref="SpResponse"/> to an <see cref="IResult"/> suitable for Minimal APIs, mapping SQL errors to HTTP status codes and error messages.
    /// </summary>
    /// <param name="response">The stored procedure response to convert.</param>
    /// <returns>An <see cref="IResult"/> representing the API response.</returns>
    public static IResult ToApiResult(SpResponse response)
    {
        if (response.IsSuccess)
        {
            return response.Content != null
                ? Results.Ok(ApiResponse<dynamic>.Success(data: response.Content, message: response.Message))
                : Results.Ok(ApiResponse.Success(message: response.Message));
        }

        var (statusCode, error) = MapSqlServerError(response.Code, response.Message);

        return Results.Json(
            ApiResponse.Failure(error.ToApiError()),
            statusCode: statusCode);
    }

    /// <summary>
    /// Maps a SQL Server error code and message to an HTTP status code and <see cref="DomainError"/>.
    /// </summary>
    /// <param name="errorCode">The SQL error code as a string.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A tuple containing the HTTP status code and a <see cref="DomainError"/>.</returns>
    private static (int statusCode, DomainError error) MapSqlServerError(string errorCode, string message)
    {
        // Try to parse SQL Server error number from error code
        if (int.TryParse(errorCode, out int sqlErrorNumber))
        {
            return MapSqlErrorNumber(sqlErrorNumber, message);
        }

        // Fallback to default internal server error
        return (StatusCodes.Status500InternalServerError,
            DomainError.Internal("SERVER_ERROR", message ?? ApiResponseHelpers.GetDefaultErrorMessage(System.Net.HttpStatusCode.InternalServerError)));
    }

    /// <summary>
    /// Maps a SQL Server error number to an HTTP status code and <see cref="DomainError"/> with a user-friendly message.
    /// </summary>
    /// <param name="sqlErrorNumber">The SQL Server error number.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A tuple containing the HTTP status code and a <see cref="DomainError"/>.</returns>
    private static (int statusCode, DomainError error) MapSqlErrorNumber(int sqlErrorNumber, string message)
    {
        return sqlErrorNumber switch
        {
            // Primary Key Violation
            2627 => (StatusCodes.Status409Conflict,
                DomainError.Conflict("DUPLICATE_KEY", "A record with this key already exists.")),

            // Unique Key Violation
            2601 => (StatusCodes.Status409Conflict,
                DomainError.Conflict("DUPLICATE_VALUE", "A record with this value already exists.")),

            // Foreign Key Violation
            547 => (StatusCodes.Status400BadRequest,
                DomainError.Validation("FOREIGN_KEY_VIOLATION", "Referenced record does not exist or cannot be deleted due to dependencies.")),

            // Null Constraint Violation
            515 => (StatusCodes.Status400BadRequest,
                DomainError.Validation("NULL_CONSTRAINT_VIOLATION", "Required field cannot be null.")),

            // Invalid Column Name
            207 => (StatusCodes.Status400BadRequest,
                DomainError.Validation("INVALID_COLUMN", "Invalid column name specified.")),

            // Invalid Object Name (Table/View not found)
            208 => (StatusCodes.Status500InternalServerError,
                DomainError.Internal("INVALID_OBJECT", "Database object not found.")),

            // Permission Denied
            229 => (StatusCodes.Status403Forbidden,
                DomainError.Forbidden("PERMISSION_DENIED", "Insufficient permissions to perform this operation.")),

            // Login Failed
            18456 => (StatusCodes.Status401Unauthorized,
                DomainError.Unauthorized("LOGIN_FAILED", "Authentication failed.")),

            // Timeout
            -2 => (StatusCodes.Status408RequestTimeout,
                DomainError.Timeout("DATABASE_TIMEOUT", "Database operation timed out.")),

            // Deadlock
            1205 => (StatusCodes.Status409Conflict,
                DomainError.Conflict("DEADLOCK_DETECTED", "Database deadlock detected. Please retry the operation.")),

            // Lock Timeout
            1222 => (StatusCodes.Status408RequestTimeout,
                DomainError.Timeout("LOCK_TIMEOUT", "Database lock timeout. Please retry the operation.")),

            // Invalid Data Type
            245 => (StatusCodes.Status400BadRequest,
                DomainError.Validation("INVALID_DATA_TYPE", "Invalid data type for the specified field.")),

            // String or Binary Data Truncation
            8152 => (StatusCodes.Status400BadRequest,
                DomainError.Validation("DATA_TRUNCATION", "Data is too long for the specified field.")),

            // Arithmetic Overflow
            8115 => (StatusCodes.Status400BadRequest,
                DomainError.Validation("ARITHMETIC_OVERFLOW", "Arithmetic overflow error.")),

            // Division by Zero
            8134 => (StatusCodes.Status400BadRequest,
                DomainError.Validation("DIVISION_BY_ZERO", "Division by zero error.")),

            // Stored Procedure Not Found
            2812 => (StatusCodes.Status500InternalServerError,
                DomainError.Internal("PROCEDURE_NOT_FOUND", "Stored procedure not found.")),

            // Invalid Parameter Count
            8144 => (StatusCodes.Status400BadRequest,
                DomainError.Validation("INVALID_PARAMETER_COUNT", "Invalid number of parameters specified.")),

            // Transaction Rollback
            3961 => (StatusCodes.Status500InternalServerError,
                DomainError.Internal("TRANSACTION_ROLLBACK", "Transaction was rolled back due to an error.")),

            // Database Full
            1105 => (StatusCodes.Status507InsufficientStorage,
                DomainError.ServiceUnavailable("DATABASE_FULL", "Database storage is full.")),

            // Log Full
            9002 => (StatusCodes.Status507InsufficientStorage,
                DomainError.ServiceUnavailable("LOG_FULL", "Database log is full.")),

            // Default case for unknown SQL errors
            _ => (StatusCodes.Status500InternalServerError,
                DomainError.Internal("SQL_ERROR", $"Database error occurred. Error code: {sqlErrorNumber}"))
        };
    }
}
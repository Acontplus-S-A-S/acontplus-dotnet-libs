namespace Acontplus.Persistence.SqlServer.Exceptions;

public static class AdoRepositoryException
{
        /// <summary>
    /// Determines if a SqlException is transient (retryable).
    /// </summary>
    public static bool IsTransientException(SqlException ex)
    {
        // Common SQL Server transient error numbers
        var transientErrorNumbers = new[] 
        { 
            4060,   // Database unavailable
            40197,  // Service busy
            40501,  // Service busy  
            40613,  // Database unavailable
            49918,  // Cannot process request
            49919,  // Cannot process request
            49920,  // Cannot process request
            4221,   // Login failed for user
            2,      // Timeout
            53,     // Network path not found
            64,     // Connection failed
            233,    // Connection failed
            10053,  // Connection aborted
            10054,  // Connection reset
            10060,  // Connection timeout
            11001   // Host not found
        };
        return transientErrorNumbers.Contains(ex.Number);
    }

    /// <summary>
    /// Maps SqlException to domain-friendly error information.
    /// </summary>
    public static SqlErrorInfo MapSqlException(SqlException ex)
    {
        return ex.Number switch
        {
            // Connection/Infrastructure errors
            2 => new SqlErrorInfo(SqlErrorType.Infrastructure, "Database operation timed out", ex),
            4060 => new SqlErrorInfo(SqlErrorType.Infrastructure, "Database is unavailable", ex),
            18456 => new SqlErrorInfo(SqlErrorType.Infrastructure, "Database authentication failed", ex),
            53 => new SqlErrorInfo(SqlErrorType.Infrastructure, "Database server not found", ex),
            
            // Constraint violations (business logic errors)
            2627 => new SqlErrorInfo(SqlErrorType.BusinessLogic, "Duplicate key violation", ex),
            547 => new SqlErrorInfo(SqlErrorType.BusinessLogic, "Foreign key constraint violation", ex),
            515 => new SqlErrorInfo(SqlErrorType.BusinessLogic, "Required field cannot be null", ex),
            
            // Custom application errors (from RAISERROR in stored procedures)
            >= 50000 => new SqlErrorInfo(SqlErrorType.BusinessLogic, ex.Message, ex),
            
            // User-defined errors from stored procedures (severity 16)
            _ when ex.Class == 16 => new SqlErrorInfo(SqlErrorType.BusinessLogic, ex.Message, ex),
            
            // All other errors as infrastructure
            _ => new SqlErrorInfo(SqlErrorType.Infrastructure, ex.Message, ex)
        };
    }

    /// <summary>
    /// Handles exceptions and determines whether to throw or return error result.
    /// </summary>
    public static void HandleSqlException(SqlException ex, ILogger logger, string operation)
    {
        var errorInfo = MapSqlException(ex);
        
        // Log the error with appropriate level
        if (errorInfo.Type == SqlErrorType.Infrastructure)
        {
            logger.LogError(ex, "Infrastructure error in {Operation}: {Message}", operation, ex.Message);
        }
        else
        {
            logger.LogWarning(ex, "Business logic error in {Operation}: {Message}", operation, ex.Message);
        }

        // Always throw - let the service layer decide how to handle business vs infrastructure errors
        throw new SqlDomainException(errorInfo.Type, errorInfo.Message, ex);
    }

}
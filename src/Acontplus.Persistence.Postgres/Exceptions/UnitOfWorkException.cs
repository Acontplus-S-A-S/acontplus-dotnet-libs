namespace Acontplus.Persistence.Postgres.Exceptions;

/// <summary>
/// Exception thrown when an error occurs within the UnitOfWork operations.
/// </summary>
public class UnitOfWorkException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWorkException"/> class.
    /// </summary>
    public UnitOfWorkException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWorkException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public UnitOfWorkException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWorkException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
    /// (Nothing in Visual Basic) if no inner exception is specified.</param>
    public UnitOfWorkException(string message, Exception innerException) : base(message, innerException)
    {
    }

    // You can add more constructors or properties if specific error codes or contexts are needed.
    // For example, if you want to categorize UoW errors:
    /*
    public UnitOfWorkException(string? message, Exception? innerException, UnitOfWorkErrorType errorType)
        : base(message, innerException)
    {
        ErrorType = errorType;
    }

    public UnitOfWorkErrorType ErrorType { get; }
    */
}

/*
// Example of an enum for specific error types, if you choose to implement it
public enum UnitOfWorkErrorType
{
    Unknown = 0,
    TransactionFailed = 1,
    RepositoryCreationFailed = 2,
    SaveChangesFailed = 3,
    NoActiveTransaction = 4,
    // ... add more as needed
}
*/

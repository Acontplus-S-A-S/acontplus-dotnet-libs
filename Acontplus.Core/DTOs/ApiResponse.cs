namespace Acontplus.Core.DTOs;

// Base class with dynamic payload
public class ApiResponse
{
    public string Code { get; set; }
    public string Message { get; set; }
    public dynamic Payload { get; set; }

    public ApiResponse() { }

    public ApiResponse(string code, string message = null, dynamic payload = null)
    {
        Code = code;
        Message = message;
        Payload = payload;
    }

    /// <summary>
    /// Creates a success response with a default code and message.
    /// </summary>
    /// <param name="code"></param> // Default is "1" to success! Excuse me
    /// <param name="payload"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static ApiResponse Success(string code = "1", dynamic payload = null, string message = "Operation successful")
    {
        return new ApiResponse(code, message, payload);
    }

    public static ApiResponse Error(string code, string message)
    {
        return new ApiResponse(code, message);
    }
}

// Generic version for strongly-typed payload
public class ApiResponse<T> : ApiResponse
{
    public new T Payload { get; set; }

    public ApiResponse() { }

    public ApiResponse(string code, string message = null, T payload = default)
        : base(code, message, null)
    {
        Payload = payload;
    }

    /// <summary>
    /// Creates a success response with a default code and message.
    /// </summary>
    /// <param name="payload"></param>
    /// <param name="code"></param> // Default is "1" to success! Excuse me
    /// <param name="message"></param>
    /// <returns></returns>
    public static ApiResponse<T> Success(T payload, string code = "1", string message = "Operation successful")
    {
        return new ApiResponse<T>(code, message, payload);
    }

    public static new ApiResponse<T> Error(string code, string message)
    {
        return new ApiResponse<T>(code, message);
    }
}

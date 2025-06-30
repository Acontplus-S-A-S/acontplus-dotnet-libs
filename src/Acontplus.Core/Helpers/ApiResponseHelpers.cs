using System.Net;

namespace Acontplus.Core.Helpers;

public static class ApiResponseHelpers
{
    public static string GetDefaultSuccessMessage(HttpStatusCode statusCode) => statusCode switch
    {
        HttpStatusCode.OK => "Operation completed successfully",
        HttpStatusCode.Created => "Resource created successfully",
        HttpStatusCode.Accepted => "Request accepted for processing",
        HttpStatusCode.NoContent => "Operation completed with no content",
        _ => "Operation completed successfully"
    };

    public static string GetDefaultErrorMessage(HttpStatusCode statusCode) => statusCode switch
    {
        HttpStatusCode.BadRequest => "Invalid request",
        HttpStatusCode.Unauthorized => "Authentication required",
        HttpStatusCode.Forbidden => "Access denied",
        HttpStatusCode.NotFound => "Resource not found",
        HttpStatusCode.Conflict => "Conflict occurred",
        HttpStatusCode.InternalServerError => "An unexpected error occurred",
        _ => "An error occurred"
    };
}
using Acontplus.Core.Constants;
using Acontplus.Core.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Acontplus.Utilities.Extensions;

public static class ApiResponseExtensions
{
    /// <summary>
    /// Converts ApiResponse to IActionResult
    /// </summary>
    public static IActionResult ToActionResult(this ApiResponse response)
    {
        return response.StatusCode switch
        {
            HttpStatusCode.OK => new OkObjectResult(response),
            HttpStatusCode.BadRequest => new BadRequestObjectResult(response),
            HttpStatusCode.Unauthorized => new UnauthorizedObjectResult(response),
            HttpStatusCode.Forbidden => new ObjectResult(response) { StatusCode = (int)HttpStatusCode.Forbidden },
            HttpStatusCode.NotFound => new NotFoundObjectResult(response),
            HttpStatusCode.Conflict => new ConflictObjectResult(response),
            HttpStatusCode.TooManyRequests => new ObjectResult(response) { StatusCode = (int)HttpStatusCode.TooManyRequests },
            HttpStatusCode.BadGateway => new ObjectResult(response) { StatusCode = (int)HttpStatusCode.BadGateway },
            _ => new ObjectResult(response) { StatusCode = (int)HttpStatusCode.InternalServerError }
        };
    }

    /// <summary>
    /// Adds pagination metadata
    /// </summary>
    public static Dictionary<string, object> WithPagination(
        this Dictionary<string, object>? metadata,
        int page,
        int pageSize,
        int totalCount)
    {
        var result = metadata ?? new Dictionary<string, object>();
        result[ApiMetadataKeys.Pagination] = new
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
            HasNextPage = page * pageSize < totalCount,
            HasPreviousPage = page > 1
        };
        return result;
    }

    /// <summary>
    /// Adds execution time metadata
    /// </summary>
    public static Dictionary<string, object> WithExecutionTime(
        this Dictionary<string, object>? metadata,
        TimeSpan executionTime)
    {
        var result = metadata ?? new Dictionary<string, object>();
        result[ApiMetadataKeys.ExecutionTime] = $"{executionTime.TotalMilliseconds}ms";
        return result;
    }
}
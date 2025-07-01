using System.Net;

namespace Acontplus.Core.Constants;

public static class HttpStatusCodes
{
    public const string Ok = "200";
    public const string Created = "201";
    public const string Accepted = "202";
    public const string NoContent = "204";
    public const string BadRequest = "400";
    public const string Unauthorized = "401";
    public const string Forbidden = "403";
    public const string NotFound = "404";
    public const string Conflict = "409";
    public const string InternalServerError = "500";

    public static string FromEnum(HttpStatusCode statusCode)
        => ((int)statusCode).ToString();
}
namespace Acontplus.Core.Constants;

/// <summary>
/// Metadata keys for debug/diagnostic information in error responses.
/// These should only be included in Development environment or when explicitly enabled.
/// </summary>
public static class DebugMetadataKeys
{
    // Debug Container
    public const string Debug = "debug";

    // Exception Details
    public const string ExceptionType = "type";
    public const string Message = "message";
    public const string StackTrace = "stackTrace";
    public const string InnerException = "innerException";
    public const string ActivityId = "activityId";

    // Additional Diagnostics
    public const string Source = "source";
    public const string TargetSite = "targetSite";
    public const string HelpLink = "helpLink";
    public const string Data = "data";
}

namespace Acontplus.Core.Constants;

/// <summary>
/// Metadata keys for health check responses and diagnostics.
/// </summary>
public static class HealthCheckMetadataKeys
{
    // Circuit Breaker States
    public const string DefaultCircuit = "default";
    public const string ApiCircuit = "api";
    public const string DatabaseCircuit = "database";
    public const string ExternalCircuit = "external";
    public const string AuthCircuit = "auth";

    // Health Check Diagnostics
    public const string LastCheckTime = "lastCheckTime";
    public const string Status = "status";
    public const string Description = "description";
    public const string Duration = "durationMs";
    public const string Exception = "exception";

    // Resource States
    public const string TotalMemory = "totalMemory";
    public const string UsedMemory = "usedMemory";
    public const string FreeMemory = "freeMemory";
    public const string CpuUsage = "cpuUsage";
    public const string ThreadCount = "threadCount";
}

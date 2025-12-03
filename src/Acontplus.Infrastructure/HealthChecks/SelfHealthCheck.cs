using System.Reflection;

namespace Acontplus.Infrastructure.HealthChecks;

/// <summary>
///     Self health check that always returns healthy with application name.
/// </summary>
public class SelfHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var appName = Assembly.GetEntryAssembly()?.GetName().Name ?? "Unknown";

        var data = new Dictionary<string, object>
        {
            ["application"] = appName,
            ["tags"] = string.Join(", ", context.Registration.Tags),
            ["lastCheckTime"] = DateTime.UtcNow
        };

        return Task.FromResult(HealthCheckResult.Healthy($"{appName} is running", data));
    }
}

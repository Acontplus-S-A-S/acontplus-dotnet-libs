namespace Acontplus.Services.HealthChecks;

/// <summary>
/// Health check for circuit breaker service.
/// </summary>
public class CircuitBreakerHealthCheck : IHealthCheck
{
    private readonly ICircuitBreakerService _circuitBreakerService;

    public CircuitBreakerHealthCheck(ICircuitBreakerService circuitBreakerService)
    {
        _circuitBreakerService = circuitBreakerService;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var defaultState = _circuitBreakerService.GetCircuitBreakerState("default");
            var apiState = _circuitBreakerService.GetCircuitBreakerState("api");
            var databaseState = _circuitBreakerService.GetCircuitBreakerState("database");
            var externalState = _circuitBreakerService.GetCircuitBreakerState("external");
            var authState = _circuitBreakerService.GetCircuitBreakerState("auth");

            var data = new Dictionary<string, object>
            {
                ["default"] = defaultState.ToString(),
                ["api"] = apiState.ToString(),
                ["database"] = databaseState.ToString(),
                ["external"] = externalState.ToString(),
                ["auth"] = authState.ToString(),
                ["lastCheckTime"] = DateTime.UtcNow
            };

            // Check if any critical circuits are open
            var criticalCircuitsOpen = new[] { databaseState, authState }.Any(state => state == CircuitBreakerState.Open);
            var anyCircuitOpen = new[] { defaultState, apiState, databaseState, externalState, authState }
                .Any(state => state == CircuitBreakerState.Open);

            if (criticalCircuitsOpen)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("Critical circuit breakers are open", data: data));
            }

            if (anyCircuitOpen)
            {
                return Task.FromResult(HealthCheckResult.Degraded("Some circuit breakers are open", data: data));
            }

            return Task.FromResult(HealthCheckResult.Healthy("All circuit breakers are operational", data));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("Circuit breaker service failed", ex));
        }
    }
}

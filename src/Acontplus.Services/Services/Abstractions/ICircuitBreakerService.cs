namespace Acontplus.Services.Services.Abstractions;

/// <summary>
/// Interface for circuit breaker service providing resilience patterns.
/// </summary>
public interface ICircuitBreakerService
{
    /// <summary>
    /// Execute an action with circuit breaker protection.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="action">The action to execute.</param>
    /// <param name="policyName">Optional policy name for specific configurations.</param>
    /// <returns>The result of the action.</returns>
    Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> action, string? policyName = null);

    /// <summary>
    /// Execute an action with circuit breaker protection.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="policyName">Optional policy name for specific configurations.</param>
    Task ExecuteAsync(Func<Task> action, string? policyName = null);

    /// <summary>
    /// Execute a synchronous action with circuit breaker protection.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="action">The action to execute.</param>
    /// <param name="policyName">Optional policy name for specific configurations.</param>
    /// <returns>The result of the action.</returns>
    TResult Execute<TResult>(Func<TResult> action, string? policyName = null);

    /// <summary>
    /// Execute a synchronous action with circuit breaker protection.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="policyName">Optional policy name for specific configurations.</param>
    void Execute(Action action, string? policyName = null);

    /// <summary>
    /// Get the current state of a circuit breaker policy.
    /// </summary>
    /// <param name="policyName">The policy name.</param>
    /// <returns>The current circuit breaker state.</returns>
    CircuitBreakerState GetCircuitBreakerState(string policyName = "default");

    /// <summary>
    /// Manually open a circuit breaker.
    /// </summary>
    /// <param name="policyName">The policy name.</param>
    void OpenCircuit(string policyName = "default");

    /// <summary>
    /// Manually close a circuit breaker.
    /// </summary>
    /// <param name="policyName">The policy name.</param>
    void CloseCircuit(string policyName = "default");
}

/// <summary>
/// Represents the state of a circuit breaker.
/// </summary>
public enum CircuitBreakerState
{
    /// <summary>
    /// Circuit is closed and allowing requests.
    /// </summary>
    Closed,

    /// <summary>
    /// Circuit is open and blocking requests.
    /// </summary>
    Open,

    /// <summary>
    /// Circuit is half-open and testing if it should close.
    /// </summary>
    HalfOpen
}

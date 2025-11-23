namespace Acontplus.Core.Abstractions.Infrastructure.Monitoring;

/// <summary>
/// Interface for metrics service.
/// </summary>
public interface IMetricsService
{
    /// <summary>
    /// Increment a counter metric.
    /// </summary>
    /// <param name="name">The name of the counter.</param>
    /// <param name="tags">Optional tags for the metric.</param>
    void IncrementCounter(string name, Dictionary<string, string>? tags = null);

    /// <summary>
    /// Record a gauge metric.
    /// </summary>
    /// <param name="name">The name of the gauge.</param>
    /// <param name="value">The value to record.</param>
    /// <param name="tags">Optional tags for the metric.</param>
    void RecordGauge(string name, double value, Dictionary<string, string>? tags = null);

    /// <summary>
    /// Record a histogram metric.
    /// </summary>
    /// <param name="name">The name of the histogram.</param>
    /// <param name="value">The value to record.</param>
    /// <param name="tags">Optional tags for the metric.</param>
    void RecordHistogram(string name, double value, Dictionary<string, string>? tags = null);
}

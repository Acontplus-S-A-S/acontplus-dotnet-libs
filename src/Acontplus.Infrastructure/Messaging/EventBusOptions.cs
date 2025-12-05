namespace Acontplus.Infrastructure.Messaging;

/// <summary>
/// Configuration options for the in-memory event bus.
/// </summary>
public class EventBusOptions
{
    /// <summary>
    /// Gets or sets whether to enable verbose diagnostic logging for event publishing and subscribing.
    /// Default is false.
    /// </summary>
    public bool EnableDiagnosticLogging { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of concurrent event handlers per event type.
    /// Default is unlimited (0).
    /// </summary>
    public int MaxConcurrentHandlers { get; set; }

    /// <summary>
    /// Gets or sets whether to dispose the event bus on application shutdown.
    /// Default is true.
    /// </summary>
    public bool DisposeOnShutdown { get; set; } = true;
}

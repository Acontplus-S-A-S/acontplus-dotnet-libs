namespace Acontplus.Core.Abstractions.Messaging;

/// <summary>
/// Defines a publisher for application-level events using in-memory channels.
/// Suitable for high-throughput, scalable event-driven architectures.
/// </summary>
public interface IEventPublisher
{
    /// <summary>
    /// Publishes an event asynchronously to all subscribers of type T.
    /// </summary>
    /// <typeparam name="T">The event type. Must be a reference type.</typeparam>
    /// <param name="eventData">The event data to publish.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task representing the asynchronous publish operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when eventData is null.</exception>
    Task PublishAsync<T>(T eventData, CancellationToken cancellationToken = default) where T : class;
}

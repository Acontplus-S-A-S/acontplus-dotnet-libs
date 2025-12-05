namespace Acontplus.Core.Abstractions.Messaging;

/// <summary>
/// Defines a subscriber for application-level events using in-memory channels.
/// Provides async enumerable streams for reactive event processing.
/// </summary>
public interface IEventSubscriber
{
    /// <summary>
    /// Subscribes to events of type T asynchronously.
    /// Returns an async enumerable stream that yields events as they are published.
    /// </summary>
    /// <typeparam name="T">The event type to subscribe to. Must be a reference type.</typeparam>
    /// <param name="cancellationToken">Cancellation token to stop the subscription.</param>
    /// <returns>An async enumerable stream of events of type T.</returns>
    IAsyncEnumerable<T> SubscribeAsync<T>(CancellationToken cancellationToken = default) where T : class;
}

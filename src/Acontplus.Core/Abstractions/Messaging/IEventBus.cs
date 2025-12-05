namespace Acontplus.Core.Abstractions.Messaging;

/// <summary>
/// Unified interface for event bus operations combining publishing and subscribing.
/// Provides a complete abstraction for in-memory event-driven communication.
/// </summary>
public interface IEventBus : IEventPublisher, IEventSubscriber
{
}

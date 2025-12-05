using Acontplus.Core.Abstractions.Messaging;
using Acontplus.TestDomain.Events;

namespace Acontplus.TestInfrastructure.EventHandlers;

/// <summary>
/// Background service that listens to OrderCreatedEvent and updates analytics/reporting.
/// Infrastructure layer - implements cross-cutting concerns.
/// </summary>
public class OrderAnalyticsHandler : BackgroundService
{
    private readonly IEventSubscriber _eventSubscriber;
    private readonly ILogger<OrderAnalyticsHandler> _logger;

    public OrderAnalyticsHandler(
        IEventSubscriber eventSubscriber,
        ILogger<OrderAnalyticsHandler> logger)
    {
        _eventSubscriber = eventSubscriber ?? throw new ArgumentNullException(nameof(eventSubscriber));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OrderAnalyticsHandler started. Listening for OrderCreatedEvent...");

        try
        {
            await foreach (var orderEvent in _eventSubscriber.SubscribeAsync<OrderCreatedEvent>(stoppingToken))
            {
                _logger.LogInformation(
                    "📊 Recording analytics for Order {OrderId} - Product: {ProductName}, Amount: ${TotalAmount}",
                    orderEvent.OrderId,
                    orderEvent.ProductName,
                    orderEvent.TotalAmount);

                // Simulate analytics processing (replace with actual analytics service)
                await Task.Delay(50, stoppingToken);

                _logger.LogInformation(
                    "✅ Analytics recorded for Order {OrderId}",
                    orderEvent.OrderId);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("OrderAnalyticsHandler is stopping.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in OrderAnalyticsHandler");
            throw;
        }
    }
}

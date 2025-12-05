using Acontplus.Core.Abstractions.Messaging;
using Acontplus.TestDomain.Events;

namespace Acontplus.TestInfrastructure.EventHandlers;

/// <summary>
/// Background service that listens to OrderCreatedEvent and sends email notifications.
/// Infrastructure layer - implements cross-cutting concerns.
/// </summary>
public class OrderNotificationHandler : BackgroundService
{
    private readonly IEventSubscriber _eventSubscriber;
    private readonly ILogger<OrderNotificationHandler> _logger;

    public OrderNotificationHandler(
        IEventSubscriber eventSubscriber,
        ILogger<OrderNotificationHandler> logger)
    {
        _eventSubscriber = eventSubscriber ?? throw new ArgumentNullException(nameof(eventSubscriber));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OrderNotificationHandler started. Listening for OrderCreatedEvent...");

        try
        {
            await foreach (var orderEvent in _eventSubscriber.SubscribeAsync<OrderCreatedEvent>(stoppingToken))
            {
                _logger.LogInformation(
                    "📧 Sending email notification for Order {OrderId} - Customer: {CustomerName}, Total: ${TotalAmount}",
                    orderEvent.OrderId,
                    orderEvent.CustomerName,
                    orderEvent.TotalAmount);

                // Simulate email sending (replace with actual email service)
                await Task.Delay(100, stoppingToken);

                _logger.LogInformation(
                    "✅ Email notification sent successfully for Order {OrderId}",
                    orderEvent.OrderId);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("OrderNotificationHandler is stopping.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in OrderNotificationHandler");
            throw;
        }
    }
}

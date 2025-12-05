using Acontplus.TestApplication.Dtos;

namespace Acontplus.TestApplication.Interfaces;

/// <summary>
/// Application service interface for order management following CQRS pattern
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// Handles CreateOrderCommand (Write operation)
    /// </summary>
    Task<Result<OrderCreatedResult>> CreateOrderAsync(
        CreateOrderCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Handles GetOrderQuery (Read operation)
    /// </summary>
    Task<Result<OrderDto>> GetOrderByIdAsync(
        GetOrderQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all orders (Read operation)
    /// </summary>
    Task<Result<IEnumerable<OrderDto>>> GetAllOrdersAsync(
        CancellationToken cancellationToken = default);
}

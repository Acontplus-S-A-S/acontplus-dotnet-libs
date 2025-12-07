namespace Demo.Api.Endpoints.Business;

/// <summary>
/// Minimal API endpoints for order management demonstrating CQRS + Event-Driven Architecture.
/// Presentation layer - thin endpoints delegating to application services.
/// </summary>
public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders")
            .WithTags("Orders - CQRS + Event Bus Demo");

        // Command: Create Order
        group.MapPost("/", async (
            [FromBody] CreateOrderCommand command,
            [FromServices] IOrderService orderService,
            CancellationToken cancellationToken) =>
        {
            var result = await orderService.CreateOrderAsync(command, cancellationToken);

            return result.Match(
                success: data => Results.Created($"/api/orders/{data.OrderId}", data),
                failure: error => Results.BadRequest(new { error = error.Message, code = error.Code }));
        })
        .WithName("CreateOrder")
        .WithSummary("Create a new order (CQRS Command)")
        .WithDescription("Creates an order and publishes OrderCreatedEvent to trigger async workflows (notifications, analytics, automation)")
        .Produces<OrderCreatedResult>(StatusCodes.Status201Created)
        .ProducesValidationProblem();

        // Query: Get Order by ID
        group.MapGet("/{orderId:int}", async (
            [FromRoute] int orderId,
            [FromServices] IOrderService orderService,
            CancellationToken cancellationToken) =>
        {
            var result = await orderService.GetOrderByIdAsync(new GetOrderQuery(orderId), cancellationToken);

            return result.Match(
                success: data => Results.Ok(data),
                failure: error => Results.NotFound(new { error = error.Message, code = error.Code }));
        })
        .WithName("GetOrder")
        .WithSummary("Get order by ID (CQRS Query)")
        .Produces<OrderDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // Query: Get All Orders
        group.MapGet("/", async (
            [FromServices] IOrderService orderService,
            CancellationToken cancellationToken) =>
        {
            var result = await orderService.GetAllOrdersAsync(cancellationToken);

            return result.Match(
                success: data => Results.Ok(data),
                failure: error => Results.BadRequest(new { error = error.Message, code = error.Code }));
        })
        .WithName("GetAllOrders")
        .WithSummary("Get all orders (CQRS Query)")
        .Produces<IEnumerable<OrderDto>>(StatusCodes.Status200OK);

        return app;
    }
}

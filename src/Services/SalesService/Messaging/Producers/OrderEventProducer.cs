using MassTransit;
using Shared.Events;
public class OrderService
{
    private readonly IPublishEndpoint _publishEndpoint;

    public OrderService(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task CreateOrder(int productId, int quantity)
    {
        var orderId = Guid.NewGuid();

        await _publishEndpoint.Publish(new OrderCreatedEvent
        {
            OrderId = orderId,
            ProductId = productId,
            Quantity = quantity
        });
    }
}
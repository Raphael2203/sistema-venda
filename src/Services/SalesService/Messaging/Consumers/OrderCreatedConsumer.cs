using MassTransit;
using Shared.Events;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedConsumer> _logger;

    public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var order = context.Message;
        _logger.LogInformation($"Pedido recebido: {order.OrderId} com o produto {order.ProductId}");

        // Simular processamento do pedido
        await Task.Delay(1000);
        _logger.LogInformation($"Pedido {order.OrderId} processado com sucesso.");
    }
}
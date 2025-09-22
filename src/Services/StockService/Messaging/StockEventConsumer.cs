using MassTransit;
using Shared.Events;
using StockService.Data;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly StockDbContext _db;

    public OrderCreatedConsumer(StockDbContext db)
    {
        _db = db;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var product = await _db.Products.FindAsync(context.Message.ProductId);
        if (product == null || product.Quantity < context.Message.Quantity)
            throw new Exception("Estoque insuficiente");

        product.Quantity -= context.Message.Quantity;
        await _db.SaveChangesAsync();
    }
}

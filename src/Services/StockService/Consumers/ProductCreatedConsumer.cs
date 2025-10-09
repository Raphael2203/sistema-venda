using Shared.Events;
using StockService.Data;
using MassTransit;

public class ProductCreatedConsumer : IConsumer<ProductCreatedEvent>
{
    private readonly StockDbContext _context;

    public ProductCreatedConsumer(StockDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<ProductCreatedEvent> context)
    {
        var message = context.Message;

        var existingProduct = await _context.Products.FindAsync(message.ProductId);
        if (existingProduct != null)
            return;

        var product = new Product
        {
            ProductId = message.ProductId,
            Name = message.Name,
            Quantity = message.Quantity
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
    }
}
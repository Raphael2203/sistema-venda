using MassTransit;
using Shared.Events;
using StockService.Data;

namespace StockService.Consumers;
public class SaleCreatedConsumer : IConsumer<SaleCreatedEvent>
{
    private readonly StockDbContext _context;

    public SaleCreatedConsumer(StockDbContext context)
    {
        _context = context;
    }
    public async Task Consume(ConsumeContext<SaleCreatedEvent> context)
    
    {
        var sale = context.Message;
        var product = _context.Products.FirstOrDefault(p => p.ProductId == sale.ProductId);

        if (product == null)
        {
            Console.WriteLine($"[Stock Service] Product {sale.ProductId} not found");
            return;
        }

        product.Quantity -= sale.Quantity;
        await _context.SaveChangesAsync();

        Console.WriteLine($"[Stock Service] Product {product.Name} updated. New quantity {product.Quantity}.");
    }
}

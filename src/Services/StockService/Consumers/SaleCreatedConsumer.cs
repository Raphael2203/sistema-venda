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
        var altered = false;

        foreach (var item in sale.Items)
        {
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product == null)
            {
                await context.Publish(new StockFailedEvent
                {
                    SaleId = sale.SaleId,
                    ProductId = item.ProductId,
                    RequestedQuantity = item.Quantity,
                    AvailableQuantity = 0,
                    Reason = "Product not found in stock"
                });

                continue;
            }

            if (product.StockQuantity < item.Quantity)
            {
                await context.Publish(new StockFailedEvent
                {
                    SaleId = sale.SaleId,
                    ProductId = product.Id,
                    RequestedQuantity = item.Quantity,
                    AvailableQuantity = product.StockQuantity,
                    Reason = "Insufficient stock"
                });
                continue;
            }
            product.StockQuantity -= item.Quantity;
            altered = true;
        }
        
        if (altered)           
            await _context.SaveChangesAsync();
    }
}

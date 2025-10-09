namespace Shared.Events;

public class SaleCreatedEvent
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
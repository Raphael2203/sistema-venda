namespace Shared.Events;

public class ProductCreatedEvent
{
    public int ProductId { get; set; }
    public string? Name { get; set; }
    public int Quantity { get; set; }
}
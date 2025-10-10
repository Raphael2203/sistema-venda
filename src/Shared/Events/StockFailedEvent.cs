namespace Shared.Events;

public class StockFailedEvent
{
    public Guid SaleId { get; set; }
    public Guid ProductId { get; set; }
    public int RequestedQuantity { get; set; }
    public int AvailableQuantity { get; set; }
    public string Reason { get; set; } = string.Empty;
}
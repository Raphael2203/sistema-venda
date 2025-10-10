using Shared.DTOs;

namespace Shared.Events;

public class SaleCreatedEvent
{
    public Guid SaleId { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime Date { get; set; }
    public List<SaleItemDto> Items { get; set; } = new();
}
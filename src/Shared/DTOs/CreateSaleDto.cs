namespace Shared.DTOs;

public class CreateSaleDto
{
    public Guid CustomerId { get; set; }
    public List<SaleItemDto> Items { get; set; } = new();
}
public class SaleItemDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }

    public string? ProductName { get; set; }
    public string? ProductDescription { get; set; }
    public decimal? ProductPrice { get; set; }
}
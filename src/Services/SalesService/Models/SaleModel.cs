namespace SalesService.Models;

public class Sale
{
    public int SaleId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTime Date { get; set; }

    public Product? Product { get; set; }
}
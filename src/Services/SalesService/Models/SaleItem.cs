using System.ComponentModel.DataAnnotations;

namespace SalesService.Models;

public class SaleItem
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SaleId { get; set; }
    public Sale? Sale { get; set; }
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? ProductDescription { get; set; }
    public decimal ProductPrice { get; set; }
    public int Quantity { get; set; }
}
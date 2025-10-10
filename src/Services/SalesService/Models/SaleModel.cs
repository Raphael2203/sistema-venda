using System.ComponentModel.DataAnnotations;

namespace SalesService.Models;

public class Sale
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CustomerId { get; set; }
    public DateTime Date { get; set; }

    public List<SaleItem> Items { get; set; } = new();
}
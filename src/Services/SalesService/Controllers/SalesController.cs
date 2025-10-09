using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;
using Shared.Events;
using SalesService.Models;

namespace SalesService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
    private readonly SalesDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public SalesController(SalesDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateSale([FromBody] SaleDto dto)
    {
        if(dto.Quantity <= 0)
            return BadRequest("Quantity must be greater than zero.");

        var product = await _context.Products.FindAsync(dto.ProductId);
        if (product == null)
            return NotFound("Product not found.");

        var sale = new Sale
        {
            ProductId = dto.ProductId,
            Quantity = dto.Quantity,
            Date = DateTime.UtcNow
        };

        _context.Sales.Add(sale);
        await _context.SaveChangesAsync();

        var saleEvent = new SaleCreatedEvent{
            ProductId = dto.ProductId,
            Quantity = dto.Quantity
        };
        await _publishEndpoint.Publish(saleEvent);

        return Ok($"Sale created and event published for Product {dto.ProductId}.");
    }

    [HttpGet("list")]
    public IActionResult ListSales()
    {
        var sales = _context.Sales
            .Include(s => s.Product)
            .Select(s => new
            {
                s.SaleId,
                s.ProductId,
                ProductName = s.Product != null ? s.Product.Name : "Unknown",
                s.Quantity,
                s.Date
            })
            .ToList();

        return Ok(sales);
    }
}
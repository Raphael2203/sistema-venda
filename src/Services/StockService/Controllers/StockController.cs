using Microsoft.AspNetCore.Mvc;
using StockService.Data;    
using Shared.DTOs;

namespace StockService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly StockDbContext _context;
    public ProductController(StockDbContext context)
    {
        _context = context;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
    {
        if (dto.StockQuantity < 0)
            return BadRequest("Stock quantity cannot be negative");
        
        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return Ok($"Product {product.Name} created with {product.StockQuantity} units in stock.");
    }


    [HttpPost("update")]
    public IActionResult UpdateProduct([FromBody] UpdateStockDto dto)
    {
        var product = _context.Products.Find(dto.ProductId);
        if (product == null)
            return NotFound("Product not found!");

        product.StockQuantity = dto.StockQuantity;
        _context.SaveChanges();

        return Ok($"Stock updated! Product: {product.Name}, Quantity: {product.StockQuantity}");
    }

    [HttpGet("list")]
    public IActionResult ListProducts()
    {
        var products = _context.Products.
        Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name ?? string.Empty,
            Description = p.Description ?? string.Empty,
            Price = p.Price,
            StockQuantity = p.StockQuantity,
            CreatedAt = p.CreatedAt
        }).ToList();
        return Ok(products);
    }
}
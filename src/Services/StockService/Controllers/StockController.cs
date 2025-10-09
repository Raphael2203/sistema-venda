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

    [HttpPost("add")]
    public IActionResult AddProduct([FromBody] ProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Quantity = dto.Quantity
        };

        _context.Products.Add(product);
        _context.SaveChanges();

        return Ok($"Product {product.Name} added");
    }


    [HttpPost("update")]
    public IActionResult UpdateProduct([FromBody] ProductDto dto)
    {
        var product = _context.Products.FirstOrDefault(s => s.ProductId == dto.ProductId);
        if (product == null)
            return NotFound("Product not found!");

        product.Quantity = dto.Quantity;
        _context.SaveChanges();

        return Ok($"Stock updated! Product: {product.ProductId}, Quantity: {product.Quantity}");
    }

    [HttpGet("list")]
    public IActionResult ListProducts()
    {
        var products = _context.Products.ToList();
        return Ok(products);
    }
}
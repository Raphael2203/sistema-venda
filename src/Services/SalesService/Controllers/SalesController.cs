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

    private readonly IHttpClientFactory _httpClientFactory;
    public SalesController(
        SalesDbContext context,
        IPublishEndpoint publishEndpoint,
        IHttpClientFactory httpClientFactory
        )
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleDto dto)
    {
        if (dto.Items == null || !dto.Items.Any())
            return BadRequest("At least one product is required.");

        var sale = new Sale
        {
            CustomerId = dto.CustomerId,
            Date = DateTime.UtcNow,
            Items = new List<SaleItem>()
        };

        var httpClient = _httpClientFactory.CreateClient("StockService");

        foreach (var itemDto in dto.Items)
        {
            if (itemDto.Quantity <= 0)
                return BadRequest("Quantity must be greater than zero.");

            var response = await httpClient.GetAsync($"/api/products/{itemDto.ProductId}");
            if (!response.IsSuccessStatusCode)
                return NotFound($"Product with ID {itemDto.ProductId} not found in Stock.");
            
            var product = await response.Content.ReadFromJsonAsync<ProductDto>();
            if (product == null)
                return BadRequest($"Failed to deserialize product {itemDto.ProductId}.");
            
            var saleItem = new SaleItem
            {
                ProductId = product.Id,
                Quantity = itemDto.Quantity,
                ProductName = product.Name,
                ProductDescription = product.Description,
                ProductPrice = product.Price,
            };

            sale.Items.Add(saleItem);
        }

        _context.Sales.Add(sale);
        await _context.SaveChangesAsync();

        var saleEvent = new SaleCreatedEvent
        {
            SaleId = sale.Id,
            CustomerId = sale.CustomerId,
            Date = sale.Date,
            Items = sale.Items.Select(i => new SaleItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                ProductDescription = i.ProductDescription,
                ProductPrice = i.ProductPrice,
                Quantity = i.Quantity
            }).ToList()
        };
        await _publishEndpoint.Publish(saleEvent);

        return Ok($"Sale created and event published for Sale {sale.Id}.");
    }

    [HttpGet("list")]
    public IActionResult ListSales()
    {
        var sales = _context.Sales
            .Include(s => s.Items)
            .Select(s => new
            {
                s.Id,
                s.CustomerId,
                s.Date,
                Items = s.Items.Select(i => new
                {
                    i.Id,
                    i.ProductId,
                    i.ProductName,
                    i.ProductDescription,
                    i.ProductPrice,
                    i.Quantity
                }).ToList()
            })
            .ToList();

        return Ok(sales);
    }
}
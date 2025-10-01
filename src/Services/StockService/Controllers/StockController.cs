using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class StockController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(new[] { "Produto 1", "Produto 2" });
    }
}
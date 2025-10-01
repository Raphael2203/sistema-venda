using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        // Exemplo de retorno estático
        return Ok(new[] { "Venda 1", "Venda 2" });
    }
}
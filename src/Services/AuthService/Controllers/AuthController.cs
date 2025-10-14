using AuthService.Models;
using AuthService.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly TokenService _tokenService;
    private readonly AuthDbContext _dbContext;

    public AuthController(TokenService tokenService, AuthDbContext dbContext)
    {
        _tokenService = tokenService;
        _dbContext = dbContext;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto login)
    {
        var user = _dbContext.Users.FirstOrDefault(u => u.Username == login.Username && u.Password == login.Password);
        if (user == null)
            return Unauthorized("Usuário ou senha inválidos");
       
        var token = _tokenService.GenerateToken(login.Username);
        return Ok(new { token });
        
    }
}

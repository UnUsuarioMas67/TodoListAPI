using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListAPI.Models;
using TodoListAPI.Services;

namespace TodoListAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLogin login)
    {
        var response = await _authService.LoginAsync(login);
        if (response == null)
            return Unauthorized("Invalid username or password");
        
        return Ok(response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegister register)
    {
        var response = await _authService.RegisterAsync(register);
        return Ok(response);
    }

    [HttpGet("test")]
    [Authorize]
    public IActionResult Test()
    {
        return Ok(new
        {
            Message = "OK", 
            Name = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value,
            Email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value,
        });
    }
}
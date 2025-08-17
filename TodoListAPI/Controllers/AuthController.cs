using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListAPI.Exceptions;
using TodoListAPI.Models;
using TodoListAPI.Services;

namespace TodoListAPI.Controllers;

[ApiController]
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
        try
        {
            var response = await _authService.LoginAsync(login);
            if (response == null)
                return Unauthorized("Invalid username or password");
        
            return Ok(response);
        }
        catch (PasswordVerificationException)
        {
            return Unauthorized("Could not verify password");
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegister register)
    {
        try
        {
            var response = await _authService.RegisterAsync(register);
            return Ok(response);
        }
        catch (DuplicateEmailException e)
        {
            return BadRequest(e.Message);
        }
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
            Id = HttpContext.User.FindFirst("id")?.Value,
        });
    }
}
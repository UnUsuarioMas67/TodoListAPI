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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(UserLogin login)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(UserRegister register)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
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
}
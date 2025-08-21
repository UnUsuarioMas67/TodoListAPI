using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using TodoListAPI.Exceptions;
using TodoListAPI.Models;

namespace TodoListAPI.Services;

public interface IAuthService
{
    Task<JwtDTO?> LoginAsync(UserLogin login);
    Task<JwtDTO?> RegisterAsync(UserRegister register);
}

public class JwtAuthService : IAuthService
{
    private readonly string _secretKey;
    private readonly IUsersService _usersService;

    public JwtAuthService(IConfiguration configuration, IUsersService usersService)
    {
        _secretKey = configuration.GetSection("Jwt:SecretKey").Value
            ?? throw new InvalidOperationException("Jwt:SecretKey not found in appsettings.json");
        _usersService = usersService;
    }

    public async Task<JwtDTO?> LoginAsync(UserLogin login)
    {
        var user = await _usersService.GetUserByEmail(login.Email);
        if (user == null)
            return null;

        try
        {
            var isPasswordValid = BCrypt.Net.BCrypt.Verify(login.Password, user.HashedPassword);
            if (!isPasswordValid)
                return null;
        }
        catch (Exception e)
        {
            if (e is SaltParseException)
                throw new PasswordVerificationException(user.UserId, e);
            throw;
        }
        
        var dto = new JwtDTO
        {
            Token = GenerateJwtToken(user)
        };
        
        return dto;
    }
    
    public async Task<JwtDTO> RegisterAsync(UserRegister register)
    {
        var user = await _usersService.AddUser(register);
        var dto = new JwtDTO
        {
            Token = GenerateJwtToken(user)
        };
        
        return dto;
    }

    private string GenerateJwtToken(UserModel user)
    {
        var ci = new ClaimsIdentity([
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("id", user.UserId.ToString())
        ]);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var handler = new JwtSecurityTokenHandler();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = ci,
            NotBefore = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = creds
        };

        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }
}
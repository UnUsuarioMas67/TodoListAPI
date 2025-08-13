using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
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
    private readonly IUserService _userService;

    public JwtAuthService(IConfiguration configuration, IUserService userService)
    {
        _secretKey = configuration.GetSection("Jwt:SecretKey").Value
            ?? throw new InvalidOperationException("Jwt:SecretKey not found in appsettings.json");
        _userService = userService;
    }

    public async Task<JwtDTO?> LoginAsync(UserLogin login)
    {
        var user = await _userService.GetUserByEmail(login.Email);
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
                return null;
            throw;
        }
        
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
            new Claim(ClaimTypes.Email, user.Email)
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

    public Task<JwtDTO?> RegisterAsync(UserRegister register)
    {
        throw new NotImplementedException();
    }
}
using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using TodoListAPI.Models;

namespace TodoListAPI.Services;

public interface IUserService
{
    Task<UserModel?> GetUserByEmail(string email);
    Task<UserModel?> GetUserById(int userId);
    Task<UserModel> AddUser(UserRegister register);
}

public class UserService : IUserService
{
    private readonly string _connectionString;

    public UserService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("TodoList")
                            ?? throw new InvalidOperationException(
                                "TodoList connection string not found in appsettings.json");
    }

    public async Task<UserModel?> GetUserByEmail(string email)
    {
        var sql = "SELECT * FROM [User] WHERE Email = @Email";

        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        return await conn.QuerySingleOrDefaultAsync<UserModel>(sql, new { Email = email });
    }

    public async Task<UserModel?> GetUserById(int userId)
    {
        var sql = "SELECT * FROM [User] WHERE UserId = @UserId";

        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        return await conn.QuerySingleOrDefaultAsync<UserModel>(sql, new { UserId = userId });
    }

    public async Task<UserModel> AddUser(UserRegister register)
    {
        var newUser = new UserModel
        {
            Name = register.Name,
            Email = register.Email,
            HashedPassword = BCrypt.Net.BCrypt.HashPassword(register.Password),
        };
        
        var sql = "INSERT INTO [User] ([Name], Email, HashedPassword) VALUES (@Name, @Email, @HashedPassword)";

        try
        {
            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            await conn.ExecuteAsync(sql, new { newUser.Name, newUser.Email, newUser.HashedPassword });
        }
        catch (SqlException e)
        {
            if (e.Number == 2627 && e.Message.Contains("Cannot insert duplicate key in object 'dbo.User'."))
                throw new ArgumentException("Duplicate email address", e);
            throw;
        }
        
        return (await GetUserByEmail(register.Email))!;
    }
}
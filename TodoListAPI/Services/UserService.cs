using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using TodoListAPI.Models;

namespace TodoListAPI.Services;

public interface IUserService
{
    Task<UserModel?> GetUserByEmail(string email);
    Task<UserModel?> GetUserById(int userId);
}

public class UserService : IUserService
{
    private readonly string _connectionString;
    
    public UserService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("TodoList")
                            ?? throw new InvalidOperationException("TodoList connection string not found in appsettings.json");
    }
    
    public async Task<UserModel?> GetUserByEmail(string email)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        return await conn.QuerySingleOrDefaultAsync<UserModel>(
            "spGetUserByEmail",
            new { Email = email },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<UserModel?> GetUserById(int userId)
    {
        var sql = "SELECT * FROM [Users] WHERE UserId = @UserId";
        
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        return await conn.QuerySingleOrDefaultAsync<UserModel>(sql,new { UserId = userId });
    }
}
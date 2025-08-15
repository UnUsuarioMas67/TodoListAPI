using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using TodoListAPI.Exceptions;
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
        var param = new
        {
            register.Name,
            register.Email,
            HashedPassword = BCrypt.Net.BCrypt.HashPassword(register.Password),
        };
        
        if (await GetUserByEmail(register.Email) != null)
            throw new DuplicateEmailException(register.Email);

        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        var createdUser = await conn.QueryFirstOrDefaultAsync<UserModel>(
            "sp_InsertAndSelectUser",
            param,
            commandType: CommandType.StoredProcedure);

        return createdUser ?? throw new DuplicateEmailException(register.Email);;
    }
}
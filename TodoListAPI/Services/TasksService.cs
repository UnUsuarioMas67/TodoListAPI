using Dapper;
using Microsoft.Data.SqlClient;
using TodoListAPI.Enums;
using TodoListAPI.Exceptions;
using TodoListAPI.Models;

namespace TodoListAPI.Services;

public interface ITasksService
{
    Task<TaskModel> CreateTaskAsync(TaskDTO dto, int userId);
    Task<TaskModel> UpdateTaskAsync(int id, TaskDTO dto);
    Task<bool> DeleteTaskAsync(int id);
    Task<PagedResult<TaskModel>> GetPagedTasksAsync(PaginationParams paginationParams);
    Task<TaskModel?> GetTaskAsync(int id);
}

public class TasksService : ITasksService
{
    private readonly string _connectionString;

    public TasksService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("TodoList") ?? throw new InvalidOperationException(
            "TodoList connection string not found in appsettings.json");
    }

    public async Task<TaskModel> CreateTaskAsync(TaskDTO dto, int userId)
    {
        var sql = """
                  IF EXISTS (SELECT * FROM [User] WHERE UserId = @UserId)
                  	BEGIN
                  		INSERT INTO Task (Title, [Description], UserId)
                  		VALUES (@Title, @Description, @UserId)

                  		SELECT t.TaskId, t.Title, t.[Description], u.UserId, u.[Name], u.Email, u.HashedPassword
                  		FROM Task t
                  		JOIN [User] u ON t.UserId = u.UserId
                  		WHERE TaskId = CAST(SCOPE_IDENTITY() AS INT)
                  	END
                  """;

        var param = new
        {
            dto.Title,
            dto.Description,
            UserId = userId
        };

        try
        {
            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var tasks = await conn.QueryAsync<TaskModel, UserModel, TaskModel>(
                sql,
                (task, user) =>
                {
                    task.Creator = user;
                    return task;
                },
                splitOn: "UserId",
                param: param);

            var task = tasks.FirstOrDefault();

            return task ?? throw new UserNotFoundException(userId);
        }
        catch (SqlException e)
        {
            if (e.Number == 547 && e.Message.Contains("dbo.User"))
                throw new UserNotFoundException(e, userId);

            throw;
        }
    }

    public async Task<TaskModel> UpdateTaskAsync(int id, TaskDTO dto)
    {
        var sql = """
                  UPDATE Task
                  SET Title = @Title, [Description] = @Description
                  WHERE TaskId = @TaskId

                  SELECT t.TaskId, t.Title, t.[Description], u.UserId, u.[Name], u.Email, u.HashedPassword
                  FROM Task t
                  JOIN [User] u ON t.UserId = u.UserId
                  WHERE TaskId = @TaskId
                  """;
        var param = new
        {
            TaskId = id,
            dto.Title,
            dto.Description,
        };
        
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        
        var tasks = await conn.QueryAsync<TaskModel, UserModel, TaskModel>(
            sql,
            (task, user) =>
            {
                task.Creator = user;
                return task;
            },
            splitOn: "UserId",
            param: param);

        return tasks.FirstOrDefault() ?? throw new TaskNotFoundException(id);
    }

    public async Task<bool> DeleteTaskAsync(int id)
    {
        var sql = "DELETE FROM Task WHERE TaskId = @TaskId";
        
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        
        var rowsAffected = await conn.ExecuteAsync(sql, new { TaskId = id });
        
        return rowsAffected != 0;
    }

    public async Task<PagedResult<TaskModel>> GetPagedTasksAsync(PaginationParams paginationParams)
    {
        var sql = """
                  SELECT t.taskid, t.title, t.description, u.userid, [name], email, hashedpassword
                  FROM Task t
                  JOIN dbo.[User] U on t.UserId = U.UserId
                  """;
        
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        
        var tasks = await conn.QueryAsync<TaskModel, UserModel, TaskModel>(
            sql,
            (task, user) =>
            {
                task.Creator = user;
                return task;
            },
            splitOn: "UserId");
        
        var sortedTasks = tasks.Where(t => 
            t.Title.Contains(paginationParams.SearchString) || t.Description.Contains(paginationParams.SearchString))
            .OrderBy(object (t) =>
            {
                return paginationParams.SortOption switch
                {
                    SortOption.Title => t.Title,
                    SortOption.Description => t.Description,
                    _ => t.TaskId
                };
            }).ToList();
        
        var pagedTasks = new PagedResult<TaskModel>(sortedTasks, paginationParams.Page, paginationParams.PageSize);
        
        return pagedTasks;
    }

    public async Task<TaskModel?> GetTaskAsync(int id)
    {
        var sql = """
                  SELECT t.taskid, t.title, t.description, u.userid, [name], email, hashedpassword
                  FROM Task t
                  JOIN dbo.[User] U on t.UserId = U.UserId
                  WHERE t.TaskId = @Id;
                  """;

        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        var tasks = await conn.QueryAsync<TaskModel, UserModel, TaskModel>(
            sql,
            (task, user) =>
            {
                task.Creator = user;
                return task;
            },
            splitOn: "UserId",
            param: new { Id = id });

        return tasks.FirstOrDefault();
    }
}
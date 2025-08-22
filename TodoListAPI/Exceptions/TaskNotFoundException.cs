namespace TodoListAPI.Exceptions;

public class TaskNotFoundException : Exception
{
    public int Id { get; }

    public TaskNotFoundException(int id)
        : base($"No task found with (ID: {id})")
    {
        Id = id;
    }
}
namespace TodoListAPI.Exceptions;

public class UserNotFoundException : Exception
{
    public int Id { get; }
    public string? Email { get; }

    public UserNotFoundException(int id , string? email = null)
        : base($"Could not find user in database (UserId: {id}).")
    {
        Id = id;
        Email = email;
    }
    
    public UserNotFoundException(Exception innerException, int id , string? email = null)
        : base($"Could not find user in database (UserId: {id}).", innerException)
    {
        Id = id;
        Email = email;
    }
}
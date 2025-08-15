namespace TodoListAPI.Exceptions;

public class PasswordVerificationException : Exception
{
    public int UserId { get; set; }
    
    public PasswordVerificationException(int userId, Exception innerException)
        : base($"Could not verify password in database (UserId: {userId})", innerException)
    {
        UserId = userId;
    }
}
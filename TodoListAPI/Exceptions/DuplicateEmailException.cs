namespace TodoListAPI.Exceptions;

public class DuplicateEmailException : Exception
{
    public string Email { get; }

    public DuplicateEmailException(string email)
        : base($"Email already registered: {email}")
    {
        Email = email;
    }
    
    public DuplicateEmailException(string email, Exception innerException)
        : base($"Email already registered: {email}", innerException)
    {
        Email = email;
    }
}
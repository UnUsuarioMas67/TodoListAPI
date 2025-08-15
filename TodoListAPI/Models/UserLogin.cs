using System.ComponentModel.DataAnnotations;

namespace TodoListAPI.Models;

public class UserLogin
{
    [Required, EmailAddress] public string Email { get; set; }
    [Required, MinLength(8)] public string Password { get; set; }
}
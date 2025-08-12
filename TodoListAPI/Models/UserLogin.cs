using System.ComponentModel.DataAnnotations;

namespace TodoListAPI.Models;

public class UserLogin
{
    public string Email { get; set; }
    public string Password { get; set; }
}
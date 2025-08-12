using System.ComponentModel.DataAnnotations;

namespace TodoListAPI.Models;

public class UserRegister
{
    public string Username { get; set; }
    [StringLength(254)] public string Email { get; set; }
    public string Password { get; set; }
}
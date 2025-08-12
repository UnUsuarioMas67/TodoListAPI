using System.ComponentModel.DataAnnotations;

namespace TodoListAPI.Models;

public class UserLogin
{
    [StringLength(254)] public string Email { get; set; }
    public string Password { get; set; }
}
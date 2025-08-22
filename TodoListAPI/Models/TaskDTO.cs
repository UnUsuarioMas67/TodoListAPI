using System.ComponentModel.DataAnnotations;

namespace TodoListAPI.Models;

public class TaskDTO
{
    [Required, StringLength(100)] public string Title { get; set; }
    [Required] public string Description { get; set; }
}
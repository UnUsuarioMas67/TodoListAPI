using System.Text.Json.Serialization;

namespace TodoListAPI.Models;

public class TaskModel
{
    [JsonPropertyName("id")] public int TaskId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    [JsonIgnore] public UserModel Creator { get; set; }
}
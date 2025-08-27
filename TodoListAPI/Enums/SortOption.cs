using System.Text.Json.Serialization;

namespace TodoListAPI.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SortOption
{
    Id,
    Title,
    Description,
}
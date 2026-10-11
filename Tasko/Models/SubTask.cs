using System.ComponentModel.DataAnnotations;

namespace Tasko.Models;

public class SubTask
{
    public int Id { get; set; }
    public int TodoItemId { get; set; }
    public TodoItem TodoItem { get; set; } = null!;
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    public bool IsDone { get; set; }
    public int SortOrder { get; set; }
}

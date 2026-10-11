using System.ComponentModel.DataAnnotations;

namespace Tasko.Models;

public class Category
{
    public int Id {  get; set; }
    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    [Required, MaxLength(7)]
    [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Use a colour like #3B82F6.")]
    public string Color { get; set; } = "#808080";
    [Required]
    public string OwnerId { get; set; } = string.Empty;
    public User Owner { get; set; } = null!;
    public ICollection<TodoItem> Todos { get; set; } = new List<TodoItem>();

}

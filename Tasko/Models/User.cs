using Microsoft.AspNetCore.Identity;

namespace Tasko.Models;

public class User : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public ICollection<TodoItem> todos { get; set; } = new List<TodoItem>();
    public ICollection<Category> Categories { get; set; } = new List<Category>();
}


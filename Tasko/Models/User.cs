using Microsoft.AspNetCore.Identity;

namespace Tasko.Models;

public class User : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}

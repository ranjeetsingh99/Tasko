using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tasko.Models;

public enum Priority
{
    Low=0, 
    Medium=1, 
    High=2
}

public class TodoItem
{
    public int Id { get; set; }
    [Required, MaxLength(100)]
    public string Title { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public bool IsDone { get; set; } = false;
    public DateTime? CompletedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public Priority Priority { get; set; } = Priority.Medium;
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
    [Required]
    public string? OwnerId { get; set; } = string.Empty;
    public User Owner { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    public ICollection<SubTask> SubTasks { get; set; } = new List<SubTask>();

    [NotMapped]
    public bool IsOverdue => !IsDone && !IsDeleted && DueDate.HasValue && DueDate.Value < DateTime.UtcNow;

    public override string ToString() =>
    $"TodoItem {{ Id={Id}, Title='{Title}', " +
    $"Notes={(Notes is null ? "null" : $"[{Notes.Length} chars]")}, " +
    $"IsDone={IsDone}, CompletedAt={CompletedAt:o}, DueDate={DueDate:o}, " +
    $"Priority={Priority}, CategoryId={CategoryId?.ToString() ?? "null"}, " +
    $"OwnerId={OwnerId}, CreatedAt={CreatedAt:o}, UpdatedAt={UpdatedAt:o}, " +
    $"IsDeleted={IsDeleted}, DeletedAt={DeletedAt:o}, " +
    $"IsOverdue={IsOverdue}, RowVersion={Convert.ToBase64String(RowVersion)}, " +
    $"SubTasks={SubTasks.Count} }}";
}



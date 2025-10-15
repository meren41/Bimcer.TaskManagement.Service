namespace Bimcer.TaskManagement.Service.Entity.Entities;

public class TaskItem
{
    public int Id { get; set; }

    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string Status { get; set; } = "pending"; // pending | in_progress | done (şimdilik string)

    public DateTime? DueDateUtc { get; set; }

    // Owner
    public int UserId { get; set; }
    public User User { get; set; } = default!;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
}

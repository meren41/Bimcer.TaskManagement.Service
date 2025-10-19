namespace Bimcer.TaskManagement.Service.Entity.Entities;

public class TaskItem
{
    public long Id { get; set; }

    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; } = false;

    // Owner
    public string UserId { get; set; } = default!;
    public User User { get; set; } = default!;

    public DateTimeOffset CreatedAt { get; set; }
}

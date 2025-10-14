namespace Bimcer.TaskManagement.Service.Entity.Entities;

public class TaskItem
{
    public long Id { get; set; }
    public string UserId { get; set; } = default!; // FK -> User
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; } = false;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

namespace Bimcer.TaskManagement.Service.Entity.DTOs.Task;

public class TaskDto
{
    public long Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

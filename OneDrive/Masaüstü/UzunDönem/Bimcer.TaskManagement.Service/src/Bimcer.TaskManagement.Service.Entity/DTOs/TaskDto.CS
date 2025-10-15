namespace Bimcer.TaskManagement.Service.Entity.DTOs.Task;

public class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string Status { get; set; } = default!;
    public DateTime? DueDateUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

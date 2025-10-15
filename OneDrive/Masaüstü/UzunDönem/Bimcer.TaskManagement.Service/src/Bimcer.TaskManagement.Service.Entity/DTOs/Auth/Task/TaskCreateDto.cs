namespace Bimcer.TaskManagement.Service.Entity.DTOs.Task;

public class TaskCreateDto
{
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public DateTime? DueDateUtc { get; set; }
}

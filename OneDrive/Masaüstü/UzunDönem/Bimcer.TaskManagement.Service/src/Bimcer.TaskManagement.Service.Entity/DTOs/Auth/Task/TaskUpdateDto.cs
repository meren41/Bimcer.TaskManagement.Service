namespace Bimcer.TaskManagement.Service.Entity.DTOs.Task;

public class TaskUpdateDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool? IsCompleted { get; set; }
}

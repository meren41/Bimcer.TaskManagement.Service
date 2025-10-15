namespace Bimcer.TaskManagement.Service.Entity.DTOs.Task;

public class TaskUpdateDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; } // optional: "pending" | "in_progress" | "done"
    public DateTime? DueDateUtc { get; set; }
}

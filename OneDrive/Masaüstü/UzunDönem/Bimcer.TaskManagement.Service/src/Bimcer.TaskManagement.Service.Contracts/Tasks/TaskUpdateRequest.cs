namespace Bimcer.TaskManagement.Service.Contracts.Tasks;

public class TaskUpdateRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool? IsCompleted { get; set; }
}


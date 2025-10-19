namespace Bimcer.TaskManagement.Service.Contracts.Tasks;

public class TaskCreateRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}


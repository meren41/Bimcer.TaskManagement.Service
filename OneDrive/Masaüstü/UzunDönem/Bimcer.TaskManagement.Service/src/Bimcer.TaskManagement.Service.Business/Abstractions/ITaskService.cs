using Bimcer.TaskManagement.Service.Contracts.Tasks;

namespace Bimcer.TaskManagement.Service.Business.Abstractions;

public interface ITaskService
{
    Task<TaskResponse> CreateAsync(string userId, TaskCreateRequest request);
    Task<IReadOnlyList<TaskResponse>> GetMyAsync(string userId);
    Task<TaskResponse> UpdateAsync(string userId, long taskId, TaskUpdateRequest request);
    Task DeleteAsync(string userId, long taskId);
}

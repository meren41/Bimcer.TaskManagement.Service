using Bimcer.TaskManagement.Service.Entity.Entities;

namespace Bimcer.TaskManagement.Service.Core.Abstractions;

public interface ITaskRepository : IEntityRepository<TaskItem>
{
    Task<List<TaskItem>> GetByUserAsync(string userId, bool asNoTracking = true);
}

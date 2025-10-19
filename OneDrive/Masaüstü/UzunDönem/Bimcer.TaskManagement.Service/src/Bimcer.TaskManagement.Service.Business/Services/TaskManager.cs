using Bimcer.TaskManagement.Service.Business.Abstractions;
using Bimcer.TaskManagement.Service.Business.Rules;
using Bimcer.TaskManagement.Service.Contracts.Tasks;
using Bimcer.TaskManagement.Service.Core.Abstractions;
using Bimcer.TaskManagement.Service.Core.Exceptions;
using Bimcer.TaskManagement.Service.Entity.Entities;

namespace Bimcer.TaskManagement.Service.Business.Services;

public sealed class TaskManager(
    ITaskRepository tasks,
    UserBusinessRules userRules) : ITaskService
{
    public async Task<TaskResponse> CreateAsync(string userId, TaskCreateRequest request)
    {
        _ = await userRules.EnsureUserExistsAsync(userId);
        TaskBusinessRules.EnsureTitleNotEmpty(request.Title);

        var entity = new TaskItem
        {
            Title = request.Title!,
            Description = request.Description,
            UserId = userId,
            IsCompleted = false,
            CreatedAt = DateTimeOffset.UtcNow
        };
        await tasks.AddAsync(entity);
        await tasks.SaveChangesAsync();

        return Map(entity);
    }

    public async Task<IReadOnlyList<TaskResponse>> GetMyAsync(string userId)
    {
        _ = await userRules.EnsureUserExistsAsync(userId);
        var list = await tasks.GetByUserAsync(userId, asNoTracking: true);
        return list.Select(Map).ToList() as IReadOnlyList<TaskResponse> ?? [];
    }

    public async Task<TaskResponse> UpdateAsync(string userId, long taskId, TaskUpdateRequest request)
    {
        _ = await userRules.EnsureUserExistsAsync(userId);
        var entity = await tasks.GetByIdAsync(taskId) ?? throw new BusinessException("Görev bulunamadı.");
        TaskBusinessRules.EnsureOwner(entity, userId);

        if (request.Title is not null)
        {
            TaskBusinessRules.EnsureTitleNotEmpty(request.Title);
            entity.Title = request.Title;
        }
        if (request.Description is not null) entity.Description = request.Description;
        if (request.IsCompleted.HasValue) entity.IsCompleted = request.IsCompleted.Value;

        tasks.Update(entity);
        await tasks.SaveChangesAsync();

        return Map(entity);
    }

    public async Task DeleteAsync(string userId, long taskId)
    {
        _ = await userRules.EnsureUserExistsAsync(userId);
        var entity = await tasks.GetByIdAsync(taskId) ?? throw new BusinessException("Görev bulunamadı.");
        TaskBusinessRules.EnsureOwner(entity, userId);

        tasks.Delete(entity);
        await tasks.SaveChangesAsync();
    }

    private static TaskResponse Map(TaskItem e) => new()
    {
        Id = e.Id,
        Title = e.Title,
        Description = e.Description ?? string.Empty,
        IsCompleted = e.IsCompleted,
        CreatedAt = e.CreatedAt
    };
}

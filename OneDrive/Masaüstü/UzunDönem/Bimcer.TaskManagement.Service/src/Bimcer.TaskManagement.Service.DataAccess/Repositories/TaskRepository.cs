using Bimcer.TaskManagement.Service.Core.Abstractions;
using Bimcer.TaskManagement.Service.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bimcer.TaskManagement.Service.DataAccess.Repositories;

/// <summary>
/// Repository for TaskItem entity with specialized queries.
/// </summary>
public class TaskRepository : EntityRepository<TaskItem>, ITaskRepository
{
    public TaskRepository(TaskManagementDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Gets all tasks for a specific user.
    /// </summary>
    public async Task<List<TaskItem>> GetByUserAsync(string userId, bool asNoTracking = true)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId cannot be null or empty.", nameof(userId));

        IQueryable<TaskItem> query = _dbSet;

        if (asNoTracking)
            query = query.AsNoTracking();

        return await query
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }
}


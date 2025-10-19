using Bimcer.TaskManagement.Service.Core.Exceptions;
using Bimcer.TaskManagement.Service.Entity.Entities;

namespace Bimcer.TaskManagement.Service.Business.Rules;

public static class TaskBusinessRules
{
    public static void EnsureTitleNotEmpty(string? title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new BusinessException("Görev başlığı boş olamaz.");
    }

    public static void EnsureOwner(TaskItem task, string userId)
    {
        if (task.UserId != userId)
            throw new BusinessException("Bu görev üzerinde yetkiniz yok.");
    }
}

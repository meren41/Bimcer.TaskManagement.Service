using Bimcer.TaskManagement.Service.Core.Abstractions;
using Bimcer.TaskManagement.Service.Core.Exceptions;
using Bimcer.TaskManagement.Service.Entity.Entities;

namespace Bimcer.TaskManagement.Service.Business.Rules;

public sealed class UserBusinessRules(IUserRepository users)
{
    public async Task<User> EnsureUserExistsAsync(string userId)
        => await users.GetByIdAsync(userId) ?? throw new BusinessException("Kullanıcı bulunamadı.");

    public async Task EnsureEmailNotTakenAsync(string email)
    {
        var existing = await users.GetByEmailAsync(email, asNoTracking: true);
        if (existing is not null) throw new BusinessException("Kullanıcı zaten kayıtlı (email).");
    }

    public async Task EnsureUsernameNotTakenAsync(string username)
    {
        var existing = await users.GetByUsernameAsync(username, asNoTracking: true);
        if (existing is not null) throw new BusinessException("Kullanıcı adı zaten alınmış.");
    }
}

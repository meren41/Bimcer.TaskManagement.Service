using Bimcer.TaskManagement.Service.Business.Abstractions;
using Bimcer.TaskManagement.Service.Contracts.Users;
using Bimcer.TaskManagement.Service.Core.Abstractions;
using Bimcer.TaskManagement.Service.Entity.Entities;

namespace Bimcer.TaskManagement.Service.Business.Services;

public sealed class UserManager(IUserRepository users) : IUserService
{
    public async Task<IReadOnlyList<UserResponse>> GetAllAsync()
    {
        var userList = await users.GetAllAsync(asNoTracking: true);
        return userList.Select(Map).ToList() as IReadOnlyList<UserResponse> ?? [];
    }

    public async Task<UserResponse?> GetByIdAsync(string userId)
    {
        var user = await users.GetByIdAsync(userId);
        return user is null ? null : Map(user);
    }

    private static UserResponse Map(User user) => new()
    {
        Id = user.Id,
        Username = user.Username,
        Email = user.Email,
        Role = user.Role,
        CreatedAt = user.CreatedAt
    };
}


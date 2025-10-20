using Bimcer.TaskManagement.Service.Contracts.Users;

namespace Bimcer.TaskManagement.Service.Business.Abstractions;

public interface IUserService
{
    Task<IReadOnlyList<UserResponse>> GetAllAsync();
    Task<UserResponse?> GetByIdAsync(string userId);
}


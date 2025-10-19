using Bimcer.TaskManagement.Service.Entity.Entities;

namespace Bimcer.TaskManagement.Service.Core.Abstractions;

public interface IUserRepository : IEntityRepository<User>
{
    Task<User?> GetByEmailAsync(string email, bool asNoTracking = true);
    Task<User?> GetByUsernameAsync(string username, bool asNoTracking = true);
}

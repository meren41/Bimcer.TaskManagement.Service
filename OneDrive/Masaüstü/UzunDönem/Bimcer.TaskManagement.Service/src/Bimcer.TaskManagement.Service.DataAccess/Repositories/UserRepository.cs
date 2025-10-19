using Bimcer.TaskManagement.Service.Core.Abstractions;
using Bimcer.TaskManagement.Service.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bimcer.TaskManagement.Service.DataAccess.Repositories;

/// <summary>
/// Repository for User entity with specialized queries.
/// </summary>
public class UserRepository : EntityRepository<User>, IUserRepository
{
    public UserRepository(TaskManagementDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Gets a user by email address.
    /// </summary>
    public async Task<User?> GetByEmailAsync(string email, bool asNoTracking = true)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or empty.", nameof(email));

        IQueryable<User> query = _dbSet;

        if (asNoTracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(u => u.Email == email);
    }

    /// <summary>
    /// Gets a user by username.
    /// </summary>
    public async Task<User?> GetByUsernameAsync(string username, bool asNoTracking = true)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be null or empty.", nameof(username));

        IQueryable<User> query = _dbSet;

        if (asNoTracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(u => u.Username == username);
    }
}


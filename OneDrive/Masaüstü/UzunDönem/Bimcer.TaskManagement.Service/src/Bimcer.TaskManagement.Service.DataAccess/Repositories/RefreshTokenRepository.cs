using Bimcer.TaskManagement.Service.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bimcer.TaskManagement.Service.DataAccess.Repositories;

/// <summary>
/// Repository for RefreshToken entity with specialized queries.
/// </summary>
public class RefreshTokenRepository : EntityRepository<RefreshToken>
{
    public RefreshTokenRepository(TaskManagementDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Gets all active refresh tokens for a user.
    /// </summary>
    public async Task<List<RefreshToken>> GetActiveTokensByUserAsync(string userId, bool asNoTracking = true)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId cannot be null or empty.", nameof(userId));

        IQueryable<RefreshToken> query = _dbSet;

        if (asNoTracking)
            query = query.AsNoTracking();

        return await query
            .Where(rt => rt.UserId == userId && rt.IsActive)
            .OrderByDescending(rt => rt.Expires)
            .ToListAsync();
    }

    /// <summary>
    /// Gets a refresh token by token string.
    /// </summary>
    public async Task<RefreshToken?> GetByTokenAsync(string token, bool asNoTracking = true)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be null or empty.", nameof(token));

        IQueryable<RefreshToken> query = _dbSet;

        if (asNoTracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(rt => rt.Token == token);
    }
}


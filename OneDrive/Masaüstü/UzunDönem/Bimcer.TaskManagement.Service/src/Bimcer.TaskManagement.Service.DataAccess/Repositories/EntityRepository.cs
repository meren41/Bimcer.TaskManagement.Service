using System.Linq.Expressions;
using Bimcer.TaskManagement.Service.Core.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Bimcer.TaskManagement.Service.DataAccess.Repositories;

/// <summary>
/// Generic repository implementation for all entities.
/// Provides base CRUD operations and common query patterns.
/// </summary>
public class EntityRepository<T> : IEntityRepository<T> where T : class
{
    protected readonly TaskManagementDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public EntityRepository(TaskManagementDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = context.Set<T>();
    }
     // CRUD Metotları
    /// <summary>
    /// Gets an entity by its ID with optional includes.
    /// </summary>
    public virtual async Task<T?> GetByIdAsync(object id, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.FirstOrDefaultAsync(e => EF.Property<object>(e, "Id").Equals(id));
    }

    /// <summary>
    /// Gets the first entity matching the predicate.
    /// </summary>
    public virtual async Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        bool asNoTracking = true,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;

        if (asNoTracking)
            query = query.AsNoTracking();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.FirstOrDefaultAsync(predicate);
    }

    /// <summary>
    /// Gets all entities with optional filtering, ordering, and includes.
    /// </summary>
    public virtual async Task<List<T>> GetAllAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool asNoTracking = true,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;

        if (asNoTracking)
            query = query.AsNoTracking();

        if (predicate != null)
            query = query.Where(predicate);

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        if (orderBy != null)
            query = orderBy(query);

        return await query.ToListAsync();
    }

    /// <summary>
    /// Adds a new entity to the database.
    /// </summary>
    public virtual async Task AddAsync(T entity, CancellationToken ct = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        await _dbSet.AddAsync(entity, ct);
    }

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    public virtual void Update(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _dbSet.Update(entity);
    }

    /// <summary>
    /// Deletes an entity.
    /// </summary>
    public virtual void Delete(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _dbSet.Remove(entity);
    }

    /// <summary>
    /// Saves all changes to the database.
    /// </summary>
    public virtual async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return await _context.SaveChangesAsync(ct);
    }
}


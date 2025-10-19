using System.Linq.Expressions;

namespace Bimcer.TaskManagement.Service.Core.Abstractions;

public interface IEntityRepository<T> where T : class
{
    // READ
    Task<T?> GetByIdAsync(object id, params Expression<Func<T, object>>[] includes);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = true,
                                 params Expression<Func<T, object>>[] includes);
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null,
                              Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                              bool asNoTracking = true,
                              params Expression<Func<T, object>>[] includes);

    // WRITE
    Task AddAsync(T entity, CancellationToken ct = default);
    void Update(T entity);
    void Delete(T entity);

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

using System.Linq.Expressions;

namespace Foxminded.StudyManager.Core.Domain.Abstractions.Repositories;

public interface IRepository<TEntity> : IOrderedQueryable<TEntity> where TEntity : class
{
    Task AddAsync(TEntity entity);
    Task AddRangeAsync(IEnumerable<TEntity> entities);
    void Delete(TEntity entity);
    void Update(TEntity entity);
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
}

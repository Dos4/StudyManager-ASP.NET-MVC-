using Foxminded.StudyManager.Core.Domain.Abstractions.Repositories;
using Foxminded.StudyManager.Infrastructure.Persistence.Repositories.IQueryableSupport;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Linq.Expressions;

namespace Foxminded.StudyManager.Infrastructure.Persistence.Repositories;

public abstract class RepositoryBase<T> : IRepository<T> where T : class
{
    public Type ElementType => typeof(T);
    public Expression Expression { get; }
    public IQueryProvider Provider { get; }

    private readonly StudyManagerDbContext? _dbContext;
    private readonly DbSet<T>? _targetDbSet;

    public RepositoryBase(StudyManagerDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException();
        _targetDbSet = _dbContext.Set<T>();
        Expression = Expression.Constant(this);
        Provider = new RepositoryBaseQueryProvider<T>(_targetDbSet);
    }

    public RepositoryBase(IQueryProvider provider, Expression expression)
    {
        Provider = provider;
        Expression = expression;
    }

    public IEnumerator<T> GetEnumerator() =>
                         Provider.Execute<IEnumerable<T>>(Expression).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public async Task AddAsync(T entity) =>await _targetDbSet!.AddAsync(entity);

    public async Task AddRangeAsync(IEnumerable<T> entities) => await _targetDbSet!.AddRangeAsync(entities);

    public void Delete(T entity) => _targetDbSet!.Remove(entity);

    public void Update(T entity) => _targetDbSet!.Entry(entity).State = EntityState.Modified;
    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
    {
        return _targetDbSet != null && await _targetDbSet.AnyAsync(predicate);
    }
}

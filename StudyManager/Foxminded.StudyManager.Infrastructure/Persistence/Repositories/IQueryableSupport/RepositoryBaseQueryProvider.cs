using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace Foxminded.StudyManager.Infrastructure.Persistence.Repositories.IQueryableSupport;

public class RepositoryBaseQueryProvider<TEntity> : IQueryProvider where TEntity : class
{
    private readonly Type _queryType;
    private readonly DbSet<TEntity> _targetDbSet;

    public RepositoryBaseQueryProvider(DbSet<TEntity> targetDbSet)
    {
        _queryType = typeof(RepositoryBase<>);
        _targetDbSet = targetDbSet ?? throw new ArgumentNullException();
    }

    public IQueryable CreateQuery(Expression expression)
    {
        try
        {
            return (IQueryable)CreateInstanceOfType(expression);
        }
        catch (TargetInvocationException targetInvocationException)
        {
            throw targetInvocationException.InnerException!;
        }
    }

    public object Execute(Expression expression)
    {
        try
        {
            MethodInfo method = typeof(RepositoryBaseQueryProvider<>).GetMethod("Execute")!;
            MethodInfo getMethod = method.MakeGenericMethod(typeof(object));

            return getMethod.Invoke(this, new[] { expression })!;
        }
        catch (TargetInvocationException targetInvocationException)
        {
            throw targetInvocationException.InnerException!;
        }
    }

    public TResult Execute<TResult>(Expression expression)
    {
        IQueryable<TEntity> newRoot = _targetDbSet;
        var treeCopier = new RootReplacingVisitor(newRoot);
        var newExpressionTree = treeCopier.Visit(expression);
        var isEnumerable = typeof(TResult).IsGenericType && typeof(TResult).GetGenericTypeDefinition() == typeof(IEnumerable<>);
        if (isEnumerable)
        {
            return (TResult)newRoot.Provider.CreateQuery(newExpressionTree);
        }
        var result = newRoot.Provider.Execute(newExpressionTree);
        return (TResult)result!;
    }

    public IQueryable<T> CreateQuery<T>(Expression expression) => (IQueryable<T>)CreateInstanceOfType(expression);

    private Object CreateInstanceOfType(Expression expression)
    {
        var elementType = expression.Type.GetGenericArguments()[0];
        var type = _queryType.MakeGenericType(elementType);
        return Activator.CreateInstance(type, new object[] { this, expression })!;
    }
}


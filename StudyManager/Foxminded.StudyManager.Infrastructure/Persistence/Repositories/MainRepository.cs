using Foxminded.StudyManager.Core.Domain.Abstractions.Repositories;

namespace Foxminded.StudyManager.Infrastructure.Persistence.Repositories;

public class MainRepository<T> : RepositoryBase<T>, IRepository<T> where T : class
{
    public MainRepository(StudyManagerDbContext dbContext) : base(dbContext)
    {
    }
}

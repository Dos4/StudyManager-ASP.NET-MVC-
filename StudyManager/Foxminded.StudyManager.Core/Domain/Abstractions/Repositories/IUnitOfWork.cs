using Foxminded.StudyManager.Core.Domain.Entities;

namespace Foxminded.StudyManager.Core.Domain.Abstractions.Repositories;

public interface IUnitOfWork
{
    public IRepository<Course> Courses { get; }
    public IRepository<Group> Groups { get; }
    public IRepository<Student> Students { get; }
    public Task<int> CompleteAsync();
}


using Foxminded.StudyManager.Core.Domain.Abstractions.Repositories;
using Foxminded.StudyManager.Core.Domain.Entities;

namespace Foxminded.StudyManager.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    public IRepository<Course> Courses { get; }
    public IRepository<Group> Groups { get; }
    public IRepository<Student> Students { get; }
    private readonly StudyManagerDbContext _context;

    public UnitOfWork(StudyManagerDbContext context, IRepository<Course> courseRepository, IRepository<Group> groupRepository,
        IRepository<Student> studentRepository)
    {
        _context = context ?? throw new ArgumentNullException();
        Courses = courseRepository ?? throw new ArgumentNullException();
        Groups = groupRepository ?? throw new ArgumentNullException();
        Students = studentRepository ?? throw new ArgumentNullException();
    }

    public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();
}


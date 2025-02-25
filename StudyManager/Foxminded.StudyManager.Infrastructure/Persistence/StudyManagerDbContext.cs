using Foxminded.StudyManager.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Foxminded.StudyManager.Infrastructure.Persistence;

public sealed class StudyManagerDbContext : DbContext
{
    public DbSet<Course> Courses { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Student> Students { get; set; }

    public StudyManagerDbContext(DbContextOptions<StudyManagerDbContext> options)
        : base(options)
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StudyManagerDbContext).Assembly);
    }
}


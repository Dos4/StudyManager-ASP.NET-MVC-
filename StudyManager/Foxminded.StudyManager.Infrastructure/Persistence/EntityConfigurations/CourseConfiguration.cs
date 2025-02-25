using Foxminded.StudyManager.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Foxminded.StudyManager.Infrastructure.Persistence.EntityConfigurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("COURSES");

        builder.HasIndex(c => new { c.Id })
            .IsUnique();

        builder.Property(c => c.Id)
            .HasColumnName("ID")
            .ValueGeneratedOnAdd();

        builder.Property(c => c.Name)
            .HasColumnName("NAME")
            .IsRequired();

        builder.Property(c => c.Description)
            .HasColumnName("DESCRIPTION");
    }
}

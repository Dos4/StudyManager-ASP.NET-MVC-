using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Foxminded.StudyManager.Core.Domain.Entities;

namespace Foxminded.StudyManager.Infrastructure.Persistence.EntityConfigurations;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.ToTable("GROUPS");

        builder.HasIndex(g => new { g.Id })
            .IsUnique();

        builder.Property(g => g.Id)
            .HasColumnName("ID")
            .ValueGeneratedOnAdd();

        builder.Property(g => g.CourseId)
            .HasColumnName("COURSE_ID")
            .IsRequired();

        builder.Property(g => g.Name)
            .HasColumnName("NAME")
            .IsRequired();

        builder.HasOne(g => g.Course);
    }
}

using Foxminded.StudyManager.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Foxminded.StudyManager.Infrastructure.Persistence.EntityConfigurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("STUDENTS");

        builder.HasIndex(s => new { s.Id })
            .IsUnique();

        builder.Property(s => s.Id)
            .HasColumnName("ID")
            .ValueGeneratedOnAdd();

        builder.Property(s => s.GroupId)
            .HasColumnName("GROUP_ID")
            .IsRequired();

        builder.Property(s => s.FirstName)
            .HasColumnName("FIRST_NAME")
            .IsRequired();

        builder.Property(s => s.LastName)
            .HasColumnName("LAST_NAME")
            .IsRequired();

        builder.HasOne(s => s.Group);
    }
}

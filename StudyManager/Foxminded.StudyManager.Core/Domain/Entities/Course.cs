namespace Foxminded.StudyManager.Core.Domain.Entities;

public class Course : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}

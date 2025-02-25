namespace Foxminded.StudyManager.Core.Domain.Entities;

public class Group : BaseEntity
{
    public required string Name { get; set; }
    public required int CourseId { get; set; }

    public virtual Course? Course { get; set; }
}

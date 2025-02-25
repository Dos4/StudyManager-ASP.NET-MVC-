namespace Foxminded.StudyManager.Core.Domain.Entities;

public class Student : BaseEntity
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required int GroupId { get; set; }

    public virtual required Group Group { get; set; }
}

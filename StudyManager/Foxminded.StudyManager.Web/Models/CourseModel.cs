namespace Foxminded.StudyManager.Web.Models;

public class CourseModel : BaseModel
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}

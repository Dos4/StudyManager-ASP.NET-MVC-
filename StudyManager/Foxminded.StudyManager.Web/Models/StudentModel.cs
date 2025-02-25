namespace Foxminded.StudyManager.Web.Models;

public class StudentModel : BaseModel
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int GroupId { get; set; }

    public string FullName => FirstName + " " + LastName;
}

using Foxminded.StudyManager.Core.Domain.Entities;

namespace Foxminded.StudyManager.Core.Application.BusinessLogic.Abstractions;

public interface ICourseService 
{
    IEnumerable<Course> GetAllCourses();
}

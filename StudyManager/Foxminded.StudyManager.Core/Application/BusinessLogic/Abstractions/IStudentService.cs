using Foxminded.StudyManager.Core.Domain.Entities;

namespace Foxminded.StudyManager.Core.Application.BusinessLogic.Abstractions;

public interface IStudentService
{
    Student Find(int id);

    IEnumerable<Student> GetAllStudents();

    IEnumerable<Student> GetStudentsForGroup(int groupId);

    Task AddStudentToDatabaseAsync(Student student);

    Task UpdateStudentAsync(Student student);

    Task DeleteStudentAsync(Student student);
}

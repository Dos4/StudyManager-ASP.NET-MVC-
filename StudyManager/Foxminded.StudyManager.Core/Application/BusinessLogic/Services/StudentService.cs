using Foxminded.StudyManager.Core.Application.BusinessLogic.Abstractions;
using Foxminded.StudyManager.Core.Application.Exceptions;
using Foxminded.StudyManager.Core.Domain.Abstractions.Repositories;
using Foxminded.StudyManager.Core.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Foxminded.StudyManager.Core.Application.BusinessLogic.Services;

public class StudentService : IStudentService
{
    private IUnitOfWork _unitOfWork;
    private ILogger<StudentService> _logger;
    private IEnumerable<Student> _query;

    public StudentService(IUnitOfWork unitOfWork, ILogger<StudentService> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException();
        _logger = logger ?? throw new ArgumentNullException();
        _query = _unitOfWork.Students ?? throw new ArgumentNullException();
    }

    public Student Find(int id) => _query.Where(s => s.Id == id).First();

    public IEnumerable<Student> GetAllStudents() => _query;

    public IEnumerable<Student> GetStudentsForGroup(int groupId) => _query.Where(s => s.GroupId == groupId);

    public async Task AddStudentToDatabaseAsync(Student student)
    {
        if (await _unitOfWork.Students.ExistsAsync(s => s.Id == student.Id))
            throw new DuplicateException();

        await _unitOfWork.Students.AddAsync(student);
        await _unitOfWork.CompleteAsync();
    }

    public async Task UpdateStudentAsync(Student student)
    {
        _unitOfWork.Students.Update(student);
        await _unitOfWork.CompleteAsync();
    }

    public async Task DeleteStudentAsync(Student student)
    {
        if(student == null) throw new ArgumentNullException();

        _unitOfWork.Students.Delete(student);
        await _unitOfWork.CompleteAsync();
    }
}

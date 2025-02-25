using Foxminded.StudyManager.Core.Application.BusinessLogic.Abstractions;
using Foxminded.StudyManager.Core.Application.BusinessLogic.Services;
using Foxminded.StudyManager.Core.Application.Exceptions;
using Foxminded.StudyManager.Core.Domain.Abstractions.Repositories;
using Foxminded.StudyManager.Core.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;

namespace Foxminded.StudyManager.Tests.CoreTests.ApplicationTests.BusinessLogicTests.ServicesTests;

[TestClass]
public class StudentServiceTests
{
    private StudentService? _studentService;
    private Mock<ILogger<StudentService>>? _loggerMock;
    private Mock<IUnitOfWork>? _unitOfWorkMock;
    private Mock<IRepository<Student>>? _repositoryMock;
    private IQueryable<Student>? _studentsList;
    private Course? _testCourse;
    private Group? _testGroup;

    [TestInitialize]
    public void Setup()
    {
        _testCourse = new Course() { Id = 23, Name = "TestCourse1" };
        _testGroup = new Group() { Id = 2, Name = "TestGroup", Course = _testCourse, CourseId = _testCourse.Id };
        _studentsList = GetTestStudents().AsQueryable();

        _repositoryMock = new Mock<IRepository<Student>>();
        _repositoryMock.Setup(g => g.Provider).Returns(_studentsList.Provider);
        _repositoryMock.Setup(g => g.Expression).Returns(_studentsList.Expression);
        _repositoryMock.Setup(g => g.ElementType).Returns(_studentsList.ElementType);
        _repositoryMock.Setup(g => g.GetEnumerator()).Returns(_studentsList.GetEnumerator());

        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(uow => uow.Students).Returns(_repositoryMock.Object);

        _loggerMock = new Mock<ILogger<StudentService>>();
        _studentService = new StudentService(_unitOfWorkMock.Object, _loggerMock.Object);
    }

    [TestMethod]
    public void GetAllStudents_ReturnsAllStudents()
    {
        var expected = _studentsList!.ToList();

        var actual = _studentService!.GetAllStudents().ToList();

        CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    [DataRow(3)]
    [DataRow(2)]
    [DataRow(5)]
    public void Find_ShouldReturnRightStudent(int testId)
    {
        var expected = _studentsList!.Where(s => s.Id == testId).First();

        var actual = _studentService!.Find(testId);

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void GetStudentsForCourse_ReturnsStudentsForGroup()
    {
        var expected = _studentsList!.Where(g => g.GroupId == _testGroup!.Id).ToList();

        var actual = _studentService!.GetStudentsForGroup(_testGroup!.Id).ToList();

        CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public async Task AddStudentToDatabase_NoDuplicate_ShouldAddStudentAsync()
    {
        var stundentToDatabase = new Student { Id = 1, FirstName = "Student", LastName = "ToDatabase"!, GroupId = _testGroup!.Id, Group = _testGroup };

        await _studentService!.AddStudentToDatabaseAsync(stundentToDatabase);

        _repositoryMock!.Verify(r => r.AddAsync(It.Is<Student>(g => g.Id == stundentToDatabase.Id)), Times.Once);
        _unitOfWorkMock!.Verify(u => u.CompleteAsync(), Times.Once);
    }

    [TestMethod]
    public async Task DeleteStudentFromDatabase_ShouldDeleteStudentAsync()
    {
        var studentToDelete = new Student { Id = 1, FirstName = "Student", LastName = "ToDelete"!, GroupId = _testGroup!.Id, Group = _testGroup };

        await _studentService!.DeleteStudentAsync(studentToDelete);

        _repositoryMock!.Verify(r => r.Delete(It.Is<Student>(g => g.Id == studentToDelete.Id)), Times.Once);
        _unitOfWorkMock!.Verify(u => u.CompleteAsync(), Times.Once);
    }

    [TestMethod]
    public async Task UpdateStudent_ShouldUpdateStudentAsync()
    {
        var updatedStudent = new Student
        {
            Id = 10,
            FirstName = "Updated",
            LastName = "Student",
            Group = _testGroup!,
            GroupId = _testGroup!.Id,
        };

        await _studentService!.UpdateStudentAsync(updatedStudent);

        _repositoryMock!.Verify(r => r.Update(It.Is<Student>(g => g.Id == updatedStudent.Id && g.FirstName == updatedStudent.FirstName 
            && g.LastName == updatedStudent.LastName)));
        _unitOfWorkMock!.Verify(u => u.CompleteAsync(), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(DuplicateException))]
    public async Task AddStudentToDatabase_IfDuplicate_ShouldThrowExceptionAsync()
    {
        _repositoryMock!.Setup(g => g.ExistsAsync(It.IsAny<Expression<Func<Student, bool>>>()))
               .ReturnsAsync(true);

        var StundentToDatabase = new Student { Id = 1, FirstName = "Student", LastName = "ToDatabase"!, GroupId = _testGroup!.Id, Group = _testGroup };

        await _studentService!.AddStudentToDatabaseAsync(StundentToDatabase);

        _repositoryMock!.Verify(r => r.AddAsync(It.Is<Student>(g => g.Id == StundentToDatabase.Id)), Times.Never);
        _unitOfWorkMock!.Verify(u => u.CompleteAsync(), Times.Never);
    }

    private IEnumerable<Student> GetTestStudents()
    {
        return new List<Student>()
        {
            new Student {Id = 1, FirstName = "TestFirstName1", LastName = "TestLastName1"!,GroupId = _testGroup!.Id, Group = _testGroup},
            new Student {Id = 2, FirstName = "TestFirstName2", LastName = "TestLastName2", GroupId = _testGroup.Id, Group = _testGroup},
            new Student {Id = 3, FirstName = "TestFirstName3", LastName = "TestLastName3", GroupId = _testGroup.Id, Group = _testGroup},
            new Student {Id = 4, FirstName = "TestFirstName4", LastName = "TestLastName4", GroupId = _testGroup.Id, Group = _testGroup},
            new Student {Id = 5, FirstName = "TestFirstName5", LastName = "TestLastName5", GroupId = _testGroup.Id, Group = _testGroup},
        };
    }
}

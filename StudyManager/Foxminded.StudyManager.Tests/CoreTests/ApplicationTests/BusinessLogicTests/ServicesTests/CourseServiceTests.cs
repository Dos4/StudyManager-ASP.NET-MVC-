using Foxminded.StudyManager.Core.Application.BusinessLogic.Services;
using Foxminded.StudyManager.Core.Domain.Abstractions.Repositories;
using Foxminded.StudyManager.Core.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Foxminded.StudyManager.Tests.CoreTests.ApplicationTests.BusinessLogicTests.ServicesTests;

[TestClass]
public class CourseServiceTests
{
    private CourseService? _courseService;
    private Mock<ILogger<CourseService>>? _loggerMock;
    private Mock<IUnitOfWork>? _unitOfWorkMock;
    private Mock<IRepository<Course>>? _repositoryMock;
    private IQueryable<Course>? _courseList;

    [TestInitialize]
    public void Setup()
    {
        _courseList = GetTestCourses().AsQueryable();

        _repositoryMock = new Mock<IRepository<Course>>();
        _repositoryMock.Setup(c => c.Provider).Returns(_courseList.Provider);
        _repositoryMock.Setup(c => c.Expression).Returns(_courseList.Expression);
        _repositoryMock.Setup(c => c.ElementType).Returns(_courseList.ElementType);
        _repositoryMock.Setup(c => c.GetEnumerator()).Returns(_courseList.GetEnumerator());

        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(uow => uow.Courses).Returns(_repositoryMock.Object);

        _loggerMock = new Mock<ILogger<CourseService>>();
        _courseService = new CourseService(_unitOfWorkMock.Object, _loggerMock.Object);
    }

    [TestMethod]
    public void GetAllCourses_ReturnsAllCourses()
    {
        var expected = _courseList!.ToList();

        var actual = _courseService!.GetAllCourses().ToList();

        CollectionAssert.AreEqual(expected, actual);
    }

    private IEnumerable<Course> GetTestCourses()
    {
        return new List<Course>()
        {
            new Course {Id = 1, Name = "TestCourse1"},
            new Course {Id = 2, Name = "TestCourse2"},
            new Course {Id = 3, Name = "TestCourse3"},
            new Course {Id = 4, Name = "TestCourse4"},
            new Course {Id = 5, Name = "TestCourse5"},
        };
    }
}

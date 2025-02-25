using Foxminded.StudyManager.Core.Application.BusinessLogic.Services;
using Foxminded.StudyManager.Core.Application.Exceptions;
using Foxminded.StudyManager.Core.Domain.Abstractions.Repositories;
using Foxminded.StudyManager.Core.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;

namespace Foxminded.StudyManager.Tests.CoreTests.ApplicationTests.BusinessLogicTests.ServicesTests;

[TestClass]
public class GroupServiceTests
{
    private GroupService? _groupService;
    private Mock<ILogger<GroupService>>? _loggerMock;
    private Mock<IUnitOfWork>? _unitOfWorkMock;
    private Mock<IRepository<Group>>? _repositoryMock;
    private IQueryable<Group>? _groupsList;
    private Course? _testCourse;

    [TestInitialize]
    public void Setup()
    {
        _testCourse = new Course() { Id = 23, Name = "TestCourse1" };
        _groupsList = GetTestGroups().AsQueryable();

        _repositoryMock = new Mock<IRepository<Group>>();
        _repositoryMock.Setup(g => g.Provider).Returns(_groupsList.Provider);
        _repositoryMock.Setup(g => g.Expression).Returns(_groupsList.Expression);
        _repositoryMock.Setup(g => g.ElementType).Returns(_groupsList.ElementType);
        _repositoryMock.Setup(g => g.GetEnumerator()).Returns(_groupsList.GetEnumerator());

        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(uow => uow.Groups).Returns(_repositoryMock.Object);

        _loggerMock = new Mock<ILogger<GroupService>>();
        _groupService = new GroupService(_unitOfWorkMock.Object, _loggerMock.Object);
    }

    [TestMethod]
    [DataRow(3)]
    [DataRow(2)]
    [DataRow(5)]
    public void Find_ShouldReturnRightGroup(int testId)
    {
        var expected = _groupsList!.Where(g=> g.Id == testId).First();

        var actual = _groupService!.Find(testId);

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void GetAllGroups_ReturnsAllCourses()
    {
        var expected = _groupsList!.ToList();

        var actual = _groupService!.GetAllGroups().ToList();

        CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void GetGroupsForCourse_ReturnsGroupsForCourse()
    {
        var expected = _groupsList!.Where(g=> g.CourseId == _testCourse!.Id).ToList();

        var actual = _groupService!.GetGroupsForCourse(_testCourse!.Id).ToList();

        CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public async Task AddGroupToDatabase_NoDuplicate_ShouldAddGroupAsync()
    {
        var groupToDatabase = new Group { Id = 100, Name = "GroupToDb1", Course = _testCourse!, CourseId = _testCourse!.Id };

        await _groupService!.AddGroupToDatabaseAsync(groupToDatabase);

        _repositoryMock!.Verify(r => r.AddAsync(It.Is<Group>(g => g.Id == groupToDatabase.Id)), Times.Once);
        _unitOfWorkMock!.Verify(u => u.CompleteAsync(), Times.Once);
    }

    [TestMethod]
    public async Task DeleteGroupFromDatabase_ShouldDeleteGroupAsync()
    {
        var groupToDelete = new Group { Id = 200, Name = "Group to Delete", CourseId = _testCourse!.Id, Course = _testCourse };

        await _groupService!.DeleteGroupAsync(groupToDelete);

        _repositoryMock!.Verify(r => r.Delete(It.Is<Group>(g => g.Id == groupToDelete.Id)), Times.Once);
        _unitOfWorkMock!.Verify(u => u.CompleteAsync(), Times.Once);
    }

    [TestMethod]
    public async Task UpdateGroup_ShouldUpdateGroupAsync()
    {
        var updatedGroup = new Group
        {
            Id = 10,
            Name = "Updated Group",
            Course = _testCourse!,
            CourseId = _testCourse!.Id,
        };

        await _groupService!.UpdateGroupAsync(updatedGroup);

        _repositoryMock!.Verify(r => r.Update(It.Is<Group>(g => g.Id == updatedGroup.Id && g.Name == updatedGroup.Name)));
        _unitOfWorkMock!.Verify(u => u.CompleteAsync(), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(DuplicateException))]
    public async Task UpdateGroup_IfDuplicateName_ShouldThrowExceptionAsync()
    {
        _repositoryMock!.Setup(g => g.ExistsAsync(It.IsAny<Expression<Func<Group, bool>>>()))
               .ReturnsAsync(true);

        var updatedGroup = new Group
        {
            Id = 10,
            Name = "Updated Group",
            Course = _testCourse!,
            CourseId = _testCourse!.Id,
        };

        await _groupService!.UpdateGroupAsync(updatedGroup);

        _repositoryMock!.Verify(r => r.Update(It.Is<Group>(g => g.Id == updatedGroup.Id)), Times.Never);
        _unitOfWorkMock!.Verify(u => u.CompleteAsync(), Times.Never);
    }

    [TestMethod]
    [ExpectedException(typeof(DuplicateException))]
    public async Task AddGroupToDatabase_IfDuplicate_ShouldThrowExceptionAsync()
    {
        _repositoryMock!.Setup(g => g.ExistsAsync(It.IsAny<Expression<Func<Group, bool>>>()))
               .ReturnsAsync(true);

        var groupToDatabase = new Group { Id = 100, Name = "Duplicate", Course = _testCourse!, CourseId = _testCourse!.Id };

        await _groupService!.AddGroupToDatabaseAsync(groupToDatabase);

        _repositoryMock!.Verify(r => r.AddAsync(It.Is<Group>(g => g.Id == groupToDatabase.Id)), Times.Never);
        _unitOfWorkMock!.Verify(u => u.CompleteAsync(), Times.Never);
    }

    private IEnumerable<Group> GetTestGroups()
    {
        return new List<Group>()
        {
            new Group {Id = 1, Name = "TestGroup1", Course = _testCourse!, CourseId = _testCourse!.Id},
            new Group {Id = 2, Name = "TestGroup2", Course = _testCourse, CourseId = _testCourse.Id},
            new Group {Id = 3, Name = "TestGroup3", Course = _testCourse, CourseId = _testCourse.Id},
            new Group {Id = 4, Name = "TestGroup4", Course = _testCourse, CourseId = _testCourse.Id},
            new Group {Id = 5, Name = "TestGroup5", Course = _testCourse, CourseId = _testCourse.Id},
        };
    }
}

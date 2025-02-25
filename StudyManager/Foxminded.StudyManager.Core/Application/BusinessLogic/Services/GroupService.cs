using Foxminded.StudyManager.Core.Application.BusinessLogic.Abstractions;
using Foxminded.StudyManager.Core.Application.Exceptions;
using Foxminded.StudyManager.Core.Domain.Abstractions.Repositories;
using Foxminded.StudyManager.Core.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Foxminded.StudyManager.Core.Application.BusinessLogic.Services;

public class GroupService : IGroupService
{
    private IUnitOfWork _unitOfWork;
    private ILogger<GroupService> _logger;
    private IEnumerable<Group> _groupQuery;
    private IEnumerable<Student> _studentsQuery;

    public GroupService(IUnitOfWork unitOfWork, ILogger<GroupService> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException();
        _logger = logger ?? throw new ArgumentNullException();
        _groupQuery = _unitOfWork.Groups ?? throw new ArgumentNullException();
        _studentsQuery = _unitOfWork.Students ?? throw new ArgumentNullException();
    }

    public Group Find(int id) => _groupQuery.Where(g => g.Id == id).First();

    public IEnumerable<Group> GetAllGroups() => _groupQuery;

    public IEnumerable<Group> GetGroupsForCourse(int courseId) => _groupQuery.Where(g => g.CourseId == courseId);

    public async Task AddGroupToDatabaseAsync(Group group)
    {
        if (await _unitOfWork.Groups.ExistsAsync(g => g.Name == group.Name))
            throw new DuplicateException();

        await _unitOfWork.Groups.AddAsync(group);
        await _unitOfWork.CompleteAsync();
    }

    public async Task UpdateGroupAsync(Group group)
    {
        if (await _unitOfWork.Groups.ExistsAsync(g => g.Name == group.Name && g.Id != group.Id))
            throw new DuplicateException();

        _unitOfWork.Groups.Update(group);
        await _unitOfWork.CompleteAsync();
    }

    public async Task DeleteGroupAsync(Group group)
    {
        if(group == null) throw new ArgumentNullException();
        CheckGroupForStudents(group);

        _unitOfWork.Groups.Delete(group);
        await _unitOfWork.CompleteAsync();
    }

    private void CheckGroupForStudents(Group group)
    {
        if (_studentsQuery.Where(s => s.GroupId == group.Id).Any())
            throw new EntityDeleteException();
    }

}

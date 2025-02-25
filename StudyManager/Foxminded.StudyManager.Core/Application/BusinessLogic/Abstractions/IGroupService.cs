using Foxminded.StudyManager.Core.Domain.Entities;

namespace Foxminded.StudyManager.Core.Application.BusinessLogic.Abstractions;

public interface IGroupService
{
    Group Find(int id);

    IEnumerable<Group> GetAllGroups();

    IEnumerable<Group> GetGroupsForCourse(int courseId);

    Task AddGroupToDatabaseAsync(Group group);

    Task UpdateGroupAsync(Group group);

    Task DeleteGroupAsync(Group group);
}

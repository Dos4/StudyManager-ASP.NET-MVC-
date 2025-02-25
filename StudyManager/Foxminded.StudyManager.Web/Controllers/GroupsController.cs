using AutoMapper;
using Foxminded.StudyManager.Core.Application.BusinessLogic.Abstractions;
using Foxminded.StudyManager.Core.Application.Exceptions;
using Foxminded.StudyManager.Core.Domain.Entities;
using Foxminded.StudyManager.Web.Models;
using Foxminded.StudyManager.Web.Models.SettingsModels;
using Foxminded.StudyManager.Web.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Foxminded.StudyManager.Web.Controllers;

public class GroupsController : Controller
{
    private ILogger<GroupsController> _logger;
    private IGroupService _groupService;
    private ICourseService _courseService;
    private IMapper _mapper;

    public GroupsController(ICourseService courseService,IGroupService groupService, IMapper mapper, ILogger<GroupsController> logger)
    {
        _groupService = groupService ?? throw new ArgumentNullException();
        _courseService = courseService ?? throw new ArgumentNullException();
        _mapper = mapper ?? throw new ArgumentNullException();
        _logger = logger ?? throw new ArgumentNullException();
    }

    public IActionResult ByCourse(int id) =>
        PartialView("_GroupListPartial", GetGroupsByCourse(id));

    public IActionResult Settings() => 
        View(GetAllGroups());

    [HttpPost]
    public async Task<IActionResult> Create(GroupSettingsModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var groupEntity = _mapper.Map<Group>(model);

        _logger.LogInformation("Starting to push group {0} with id {1}, to database", groupEntity.Name, groupEntity.Id);
        await _groupService.AddGroupToDatabaseAsync(groupEntity);
        _logger.LogInformation("Group {0} {1} succesfull added to database", groupEntity.Name, groupEntity.Id);

        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> UpdateAsync(Group model)
    {
        _logger.LogInformation("Starting to update group {0}", model.Id);
        await _groupService.UpdateGroupAsync(model);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            _logger.LogInformation("Starting to find group with id {0}", id);
            var group = _groupService.Find(id);

            await _groupService.DeleteGroupAsync(group);
            return Ok();
        }
        catch (EntityDeleteException exception)
        {
            _logger.LogError(exception.StackTrace);
            return StatusCode(500, InterfaceText.StudentsInGroupExceptionText);
        }
        catch (ArgumentNullException exception)
        {
            _logger.LogError("Group with id {0} was not fond in database. {1}", id, exception.StackTrace);
            return NotFound((InterfaceText.NotFoundGroupText, id));
        }
    }

    [HttpPost]
    public IActionResult GetForm([FromBody] FormRequest request)
    {
        string mode = request.Mode!;
        int? id = request.Id;
        _logger.LogInformation("Recieved button mode: {0}, and entity id: {1}", mode, id);

        ViewBag.Action = mode;
        ViewBag.Courses = GetAllCourses();

        if (mode == "edit" && id.HasValue)
        {
            var group = _groupService.Find((int)id);
            return PartialView("_GroupFormPartial", _mapper.Map<GroupModel>(group));
        }
        return PartialView("_GroupFormPartial", new GroupModel());
    }

    public IActionResult GetEntity(int id)
    {
        var group = _groupService.Find(id);
        return Json(new
        {
            group.Name,
            group.CourseId
        });
    }

    private IEnumerable<GroupModel> GetAllGroups()
    {
        var groups = _groupService.GetAllGroups();
        _logger.LogInformation("Groups were received: {0}", groups.Count());

        return _mapper.Map<IEnumerable<GroupModel>>(groups);
    }

    private IEnumerable<CourseModel> GetAllCourses()
    {
        var courses = _courseService.GetAllCourses();
        _logger.LogInformation("Courses were received: {0}", courses.Count());

        return _mapper.Map<IEnumerable<CourseModel>>(courses);
    }

    private IEnumerable<GroupModel> GetGroupsByCourse(int id)
    {
        _logger.LogInformation("Trying to get groups for course with id: {0}", id);
        var groups = _groupService.GetGroupsForCourse(id);
        _logger.LogInformation("Groups were received: {0} for course id: {1}", groups.Count(), id);
        
        return _mapper.Map<IEnumerable<GroupModel>>(groups);
    }
}

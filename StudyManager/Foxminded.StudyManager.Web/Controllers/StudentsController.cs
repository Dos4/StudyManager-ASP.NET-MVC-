using AutoMapper;
using Foxminded.StudyManager.Core.Application.BusinessLogic.Abstractions;
using Foxminded.StudyManager.Core.Domain.Entities;
using Foxminded.StudyManager.Web.Models;
using Foxminded.StudyManager.Web.Models.SettingsModels;
using Foxminded.StudyManager.Web.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Foxminded.StudyManager.Web.Controllers;

public class StudentsController : Controller
{
    private ILogger<StudentsController> _logger;
    private IStudentService _studentService;
    private IGroupService _groupService;
    private IMapper _mapper;

    public StudentsController(IStudentService studentService, IGroupService groupService, IMapper mapper, ILogger<StudentsController> logger)
    {
        _studentService = studentService ?? throw new ArgumentNullException();
        _groupService = groupService ?? throw new ArgumentNullException();
        _mapper = mapper ?? throw new ArgumentNullException();
        _logger = logger ?? throw new ArgumentNullException();
    }

    public IActionResult ByGroup(int id) =>
        PartialView("_StudentListPartial", GetStudentsForGroup(id));

    public IActionResult Settings() => 
        View(GetAllStudents());

    [HttpPost]
    public async Task<IActionResult> Create(StudentSettingsModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var studentEntity = _mapper.Map<Student>(model);

        _logger.LogInformation("Starting to push student {0} {1} with id {2}, to database", studentEntity.FirstName, studentEntity.LastName, studentEntity.Id);
        await _studentService.AddStudentToDatabaseAsync(studentEntity);
        _logger.LogInformation("Student {0} {1} {2} succesfull added to database", studentEntity.FirstName, studentEntity.LastName, studentEntity.Id);

        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Update(Student model)
    {
        _logger.LogInformation("Starting to update student {0}", model.Id);
        await _studentService.UpdateStudentAsync(model);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            _logger.LogInformation("Starting to find student with id {0}", id);
            var student = _studentService.Find(id);
           
            await _studentService.DeleteStudentAsync(student);
            return Ok();
        }
        catch (ArgumentNullException exception)
        {
            _logger.LogError("Student with id {0} was not fond in database. {1}", id, exception.StackTrace);
            return NotFound((InterfaceText.NotFoundStudentText, id));
        }
    }

    [HttpPost]
    public IActionResult GetForm([FromBody] FormRequest request)
    {
        string mode = request.Mode!;
        int? id = request.Id;
        _logger.LogInformation("Recieved button mode: {0}, and entity id: {1}", mode, id);

        ViewBag.Action = mode;
        ViewBag.Groups = GetAllGroups();
        if (mode == "edit" && id.HasValue)
        {
            var student = _studentService.Find((int)id);
            return PartialView("_StudentFormPartial", _mapper.Map<StudentModel>(student));
        }
        return PartialView("_StudentFormPartial", new StudentModel());
    }

    public IActionResult GetEntity(int id)
    {
        var student = _studentService.Find(id);
        return Json(new
        {
            student.FirstName,
            student.LastName,
            student.GroupId,
        });
    }

    private IEnumerable<GroupModel> GetAllGroups()
    {
        var groups = _groupService.GetAllGroups();
        _logger.LogInformation("Groups were received: {0}", groups.Count());

        return _mapper.Map<IEnumerable<GroupModel>>(groups);
    }

    private IEnumerable<StudentModel> GetAllStudents()
    {
        var students = _studentService.GetAllStudents();
        _logger.LogInformation("Students were received: {0}", students.Count());

        return _mapper.Map<IEnumerable<StudentModel>>(students);
    }

    private IEnumerable<StudentModel> GetStudentsForGroup(int id)
    {
        _logger.LogInformation("Trying to get students for group id: {0}", id);
        var students = _studentService.GetStudentsForGroup(id);
        _logger.LogInformation("Students were received {0} for group id: {1}", students.Count(), id);

        return _mapper.Map<IEnumerable<StudentModel>>(students);
    }
}

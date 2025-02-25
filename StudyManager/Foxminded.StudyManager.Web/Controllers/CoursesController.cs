using AutoMapper;
using Foxminded.StudyManager.Core.Application.BusinessLogic.Abstractions;
using Foxminded.StudyManager.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Foxminded.StudyManager.Web.Controllers
{
    public class CoursesController : Controller
    {
        private ILogger<CoursesController> _logger;
        private ICourseService _courseService;
        private IMapper _mapper;

        public CoursesController(ICourseService courseService, IMapper mapper, ILogger<CoursesController> logger)
        {
            _courseService = courseService ?? throw new ArgumentNullException();
            _mapper = mapper ?? throw new ArgumentNullException();
            _logger = logger ?? throw new ArgumentNullException();
        }

        public IActionResult CoursesPartial() =>
             PartialView("_CourseListPartial", GetCourses());

        private IEnumerable<CourseModel> GetCourses()
        {
            var courses = _courseService.GetAllCourses();
            _logger.LogInformation("Courses were recieved: {0}", courses.Count());
            return _mapper.Map<IEnumerable<CourseModel>>(courses);
        }
    }
}

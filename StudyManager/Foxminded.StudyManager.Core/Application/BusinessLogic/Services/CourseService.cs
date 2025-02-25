using Foxminded.StudyManager.Core.Application.BusinessLogic.Abstractions;
using Foxminded.StudyManager.Core.Domain.Abstractions.Repositories;
using Foxminded.StudyManager.Core.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Foxminded.StudyManager.Core.Application.BusinessLogic.Services;

public class CourseService : ICourseService
{
    private IUnitOfWork _unitOfWork;
    private ILogger<CourseService> _logger;
    private IEnumerable<Course> _query;

    public CourseService(IUnitOfWork unitOfWork, ILogger<CourseService> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException();
        _logger = logger ?? throw new ArgumentNullException();
        _query = _unitOfWork.Courses ?? throw new ArgumentNullException();
    }

    public IEnumerable<Course> GetAllCourses() => _query;
}

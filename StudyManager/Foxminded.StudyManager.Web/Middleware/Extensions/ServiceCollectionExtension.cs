using Foxminded.StudyManager.Core.Application.BusinessLogic.Abstractions;
using Foxminded.StudyManager.Core.Application.BusinessLogic.Services;
using Foxminded.StudyManager.Core.Domain.Abstractions.Repositories;
using Foxminded.StudyManager.Core.Domain.Entities;
using Foxminded.StudyManager.Infrastructure.Persistence;
using Foxminded.StudyManager.Infrastructure.Persistence.Repositories;
using Foxminded.StudyManager.Web.Models.Mappers;

namespace Foxminded.StudyManager.Web.Middleware.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        return services.AddLogging()
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<IRepository<Course>, MainRepository<Course>>()
            .AddScoped<IRepository<Group>, MainRepository<Group>>()
            .AddScoped<IRepository<Student>, MainRepository<Student>>()
            .AddTransient<ICourseService, CourseService>()
            .AddTransient<IGroupService, GroupService>()
            .AddTransient<IStudentService, StudentService>()
            .AddAutoMapper(typeof(MappingProfile));
    }
}

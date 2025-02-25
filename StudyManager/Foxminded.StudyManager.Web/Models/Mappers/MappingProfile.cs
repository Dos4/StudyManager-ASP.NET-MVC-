using AutoMapper;
using Foxminded.StudyManager.Core.Domain.Entities;
using Foxminded.StudyManager.Web.Models.SettingsModels;

namespace Foxminded.StudyManager.Web.Models.Mappers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Course, CourseModel>();

        CreateMap<Student, StudentModel>();

        CreateMap<Group, GroupModel>();

        CreateMap<GroupSettingsModel, Group>();

        CreateMap<StudentSettingsModel, Student>();
    }
}

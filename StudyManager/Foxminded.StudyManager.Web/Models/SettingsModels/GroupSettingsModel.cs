using System.ComponentModel.DataAnnotations;

namespace Foxminded.StudyManager.Web.Models.SettingsModels;

public class GroupSettingsModel
{
    [Required]
    public required string Name { get; set; }

    [Required]
    public required int CourseId { get; set; }
}

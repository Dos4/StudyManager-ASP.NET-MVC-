using System.ComponentModel.DataAnnotations;

namespace Foxminded.StudyManager.Web.Models.SettingsModels;

public class StudentSettingsModel
{
    [Required]
    public required string FirstName { get; set; }

    [Required]
    public required string LastName { get; set; }

    [Required]
    public required int GroupId { get; set; }
}

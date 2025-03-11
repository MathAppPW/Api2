using System.ComponentModel.DataAnnotations;

namespace MathAppApi.Features.UserProfile.Dtos;

public class ExperienceResponse
{
    [Required]
    public int Level { get; set; } = 0;
    [Required]
    public int Experience { get; set; } = 0;
    [Required]
    public bool LeveledUp { get; set; } = false;
}

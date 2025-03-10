using System.ComponentModel.DataAnnotations;

namespace MathAppApi.Features.UserProfile.Dtos;

public class ExperienceDto
{
    [Required]
    [StringLength(128)]
    public string UserId { get; set; } = "";

    [Required]
    public int Amount { get; set; } = 0;
}

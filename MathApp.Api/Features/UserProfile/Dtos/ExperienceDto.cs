using System.ComponentModel.DataAnnotations;

namespace MathAppApi.Features.UserProfile.Dtos;

public class ExperienceDto
{
    [Required]
    public int Amount { get; set; } = 0;
}

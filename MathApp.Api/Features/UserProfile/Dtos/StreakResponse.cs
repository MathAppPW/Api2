using System.ComponentModel.DataAnnotations;

namespace MathAppApi.Features.UserProfile.Dtos;

public class StreakResponse
{
    [Required]
    public int Streak { get; set; } = 0;
}

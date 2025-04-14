using System.ComponentModel.DataAnnotations;

namespace MathAppApi.Features.UserProfile.Dtos;

public class AchievementsResponse
{
    [Required]
    public List<bool> IsUnlocked { get; set; } = new();
}

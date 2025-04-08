using System.ComponentModel.DataAnnotations;

namespace MathAppApi.Features.Friends.Dtos;

public class ProfileSearchResponse
{
    [Required]
    public int Level { get; set; } = 0;
    [Required]
    public int ProfileSkin { get; set; } = 0;
    [Required]
    public int RocketSkin { get; set; } = 0;
    [Required]
    public int Streak { get; set; } = 0;
    [Required]
    public int MaxStreak { get; set; } = 0;
    [Required]
    public int ExercisesCompleted { get; set; } = 0;
    [Required]
    public bool IsFriend { get; set; } = false;
}

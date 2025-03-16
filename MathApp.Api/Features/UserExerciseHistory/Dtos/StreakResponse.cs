using System.ComponentModel.DataAnnotations;

namespace MathAppApi.Features.UserExerciseHistory.Dtos;

public class StreakResponse
{
    [Required]
    public int Streak { get; set; }
    [Required]
    public DateTime Start { get; set; }
    [Required]
    public DateTime End { get; set; }
}

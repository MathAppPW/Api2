using System.ComponentModel.DataAnnotations;

namespace MathAppApi.Features.UserExerciseHistory.Dtos;

public class TimeSpentDto
{
    [Required]
    public DateTime Date { get; set; }
    [Required]
    public int SecondsSpent { get; set; }
}

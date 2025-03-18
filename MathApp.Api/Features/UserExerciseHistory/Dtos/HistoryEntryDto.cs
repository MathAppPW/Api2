using System.ComponentModel.DataAnnotations;

namespace MathAppApi.Features.UserExerciseHistory.Dtos;

public class HistoryEntryDto
{
    [Required]
    public string ExerciseId { get; set; } = "";
    [Required]
    public DateTime Date { get; set; }
    [Required]
    public int TimeSpent { get; set; }
    [Required]
    public bool Success { get; set; }
}

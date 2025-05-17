using System.ComponentModel.DataAnnotations;

namespace MathAppApi.Features.UserExerciseHistory.Dtos;

public class HistoryEntryDto
{
    [Required]
    public int SeriesId { get; set; }
    [Required]
    public DateTime Date { get; set; }
    [Required]
    public int TimeSpent { get; set; }
    [Required]
    public int SuccessfulCount { get; set; }
    [Required]
    public int FailedCount { get; set; }
}

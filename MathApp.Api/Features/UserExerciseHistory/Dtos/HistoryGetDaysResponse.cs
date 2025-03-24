using System.ComponentModel.DataAnnotations;

namespace MathAppApi.Features.UserExerciseHistory.Dtos;

public class HistoryGetDaysResponse
{
    [Required]
    public List<HistoryGetDaysResponseDay> Days { get; set; } = [];
}

public class HistoryGetDaysResponseDay
{
    [Required]
    public DateTime Date { get; set; }
    [Required]
    public int SecondsSpent { get; set; }
    [Required]
    public int ExercisesCount { get; set; }
    [Required]
    public int ExercisesCountSuccessful { get; set; }
    [Required]
    public int ExercisesCountFailed { get; set; }

}

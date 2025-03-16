using System.ComponentModel.DataAnnotations;

namespace MathAppApi.Features.UserExerciseHistory.Dtos;

public class ExerciseCounterDto
{
    [Required]
    public DateTime Date { get; set; }
    [Required]
    public int Successful { get; set; }
    [Required]
    public int Failed { get; set; }
    [Required]
    public int Total { get; set; }
}

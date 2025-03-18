using System.ComponentModel.DataAnnotations;

namespace MathAppApi.Features.UserExerciseHistory.Dtos;

public class HistoryGetDto
{
    [Required]
    public string Id { get; set; } = "";
}

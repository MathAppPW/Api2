using System.ComponentModel.DataAnnotations;

namespace MathAppApi.Features.UserExerciseHistory.Dtos;

public class HistoryGetAllResponse
{
    [Required]
    public List<Models.UserHistoryEntry> Entries { get; set; } = [];
}

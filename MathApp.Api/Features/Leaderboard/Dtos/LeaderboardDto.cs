using System.ComponentModel.DataAnnotations;

namespace MathAppApi.Features.Leaderboard.Dtos;

public class LeaderboardDto
{
    [Required]
    public string Name { get; set; } = "";
    [Required]
    public int Days { get; set; }
}

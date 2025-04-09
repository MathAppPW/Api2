namespace MathAppApi.Features.Leaderboard.Dtos;

public class LeaderboardEntry
{
    public Models.UserProfile User { get; set; } = new();
    public int Score {  get; set; }
}

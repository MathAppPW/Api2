namespace MathAppApi.Features.Leaderboard.Dtos;

public class LeaderboardResponse
{
    public List<Models.LeaderboardEntry> Entries { get; set; } = [];
    public int MyPosition {  get; set; }
}

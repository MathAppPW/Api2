using MathAppApi.Features.Leaderboard.Dtos;

namespace MathAppApi.Features.Leaderboard.Services.Interfaces;

public interface ILeaderboardService
{
    public Task<List<Models.LeaderboardEntry>> FilterByFriends(List<Models.LeaderboardEntry> entries, string userId);
    public Task UpdateLeaderboard(LeaderboardDto dto);
}

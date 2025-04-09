using MathApp.Dal.Interfaces;
using MathAppApi.Features.Leaderboard.Dtos;
using MathAppApi.Features.Leaderboard.Services.Interfaces;
using MathAppApi.Shared.Utils.Interfaces;
using Models;

namespace MathAppApi.Features.Leaderboard.Services;

public class LeaderboardService : ILeaderboardService
{
    private readonly ILeaderboardRepo _leaderboardRepo;
    private readonly IUserProfileRepo _userProfileRepo;
    private readonly IFriendshipRepo _friendshipRepo;

    private readonly IHistoryUtils _historyUtils;

    public LeaderboardService(ILeaderboardRepo leaderboardRepo, IUserProfileRepo userProfileRepo, IFriendshipRepo friendshipRepo, IHistoryUtils historyUtils)
    {
        _leaderboardRepo = leaderboardRepo;
        _userProfileRepo = userProfileRepo;
        _friendshipRepo = friendshipRepo;
        _historyUtils = historyUtils;
    }

    public async Task<List<Models.LeaderboardEntry>> FilterByFriends(List<Models.LeaderboardEntry> entries, string userId)
    {
        var friendScores = new List<Models.LeaderboardEntry>();

        foreach (var entry in entries)
        {
            var friendship = await _friendshipRepo.FindOneAsync(fr =>
            fr.UserId1 == userId && fr.UserId2 == entry.User.Id || fr.UserId1 == entry.User.Id && fr.UserId2 == userId);
            if(friendship != null || entry.User.Id == userId)
            {
                friendScores.Add(entry);
            }
        }

        return friendScores;
    }

    public async Task UpdateLeaderboard(LeaderboardDto dto)
    {
        var leaderboard = await _leaderboardRepo.FindOneAsync(e => e.Id == dto.Name);
        if(leaderboard == null)
        {
            leaderboard = new Models.Leaderboard
            {
                Id = dto.Name,
                Entries = await GetScores(dto.Days)
            };
            await _leaderboardRepo.AddAsync(leaderboard);
        }
        else
        {
            leaderboard.Entries = await GetScores(dto.Days);
            await _leaderboardRepo.UpdateAsync(leaderboard);
        }
    }

    private async Task<List<Models.LeaderboardEntry>> GetScores(int days)
    {
        var users = await _userProfileRepo.GetAllAsync();
        var today = DateTime.Today;
        var fromDate = today.AddDays(-days);

        var entries = new List<Models.LeaderboardEntry>();
        foreach(var user in users)
        {
            var activity = await _historyUtils.GetActivityPerDay(user);
            if (activity == null)
                continue;

            var score = activity
            .Where(day => day.Date >= fromDate && day.Date < today)
            .Sum(day => day.ExercisesCountSuccessful);

            entries.Add(new Models.LeaderboardEntry
            {
                User = user,
                Score = score
            });
        }

        return entries
        .OrderByDescending(u => u.Score)
        .ToList();
    }
}

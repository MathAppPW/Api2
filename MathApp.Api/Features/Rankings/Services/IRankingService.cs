using MathAppApi.Features.Rankings.Dtos;

namespace MathAppApi.Features.Rankings.Services;

public interface IRankingService
{
    public Task<Ranking> GetGlobalRankingAsync(int count, string userId);
    public Task<Ranking> GetFriendsRankingAsync(string userId, int count);
}
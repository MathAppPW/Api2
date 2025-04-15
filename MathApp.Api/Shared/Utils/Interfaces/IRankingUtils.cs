using Models;

namespace MathAppApi.Shared.Utils.Interfaces;

public interface IRankingUtils
{
    Task<int> GetGlobalRanking(string userId);
    Task<int> GetFriendsRanking(string sourceId, string userId);
    Task<List<UserProfile>> GetFriendsRanking(string userId);
    Task<List<UserProfile>> GetGlobalTop10();
}
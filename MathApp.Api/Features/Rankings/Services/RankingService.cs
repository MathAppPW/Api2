using Dal;
using MathAppApi.Features.Rankings.Dtos;
using Microsoft.EntityFrameworkCore;

namespace MathAppApi.Features.Rankings.Services;

public class RankingService : IRankingService
{
    private readonly MathAppDbContext _db;

    public RankingService(MathAppDbContext db)
    {
        _db = db;
    }

    public async Task<Ranking> GetGlobalRankingAsync(int count)
    {
        var today = DateTime.UtcNow;
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var rankingEntries = await _db.Database
            .SqlQueryRaw<RankingEntry>(
                """
                        SELECT LIMIT {0} u.Username, up.AvatarId, COUNT(*) as Score
                        FROM "Users" u 
                        JOIN "UserProfiles" up on u.Id = up.UserId
                        CROSS JOIN UNNEST(up.History) AS his
                        JOIN "UserHistoryEntries" uhe on uhe.Id = his.Id
                        WHERE uhe.Date >= {1}
                        GROUP BY u.Username, up.AvatarId
                        ORDER BY Score DESC
                """, count, monthStart
            ).ToListAsync();
        return new Ranking { RankingEntries = rankingEntries };
    }

    public async Task<Ranking> GetFriendsRankingAsync(string userId, int count)
    {
        var today = DateTime.UtcNow;
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var rankingEntries = await _db.Database
            .SqlQueryRaw<RankingEntry>(
                """
                    SELECT LIMIT {1} u2.Username, up.AvatarId, COUNT(*) as Score
                    FROM "Users" u
                    JOIN "Friendships" f on f.UserId1 = {0} OR f.UserId2 = {0}
                    JOIN "Users" u2 on (f.UserId1 = u2.Id OR f.UserId2 = u2.Id) AND u2.Id != {0}
                    JOIN "UserProfiles" up on u2.Id = up.UserId
                    CROSS JOIN UNNEST(up.History) AS his
                    JOIN "UserHistoryEntries" uhe on uhe.Id = his.Id
                    WHERE uhe.Date >= {2}
                    GROUP BY u2.Username, up.AvatarId
                    ORDER BY SCORE DESC
                """, userId, count, monthStart
            ).ToListAsync();
        return new Ranking { RankingEntries = rankingEntries };
    }
}
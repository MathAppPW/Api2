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

    public async Task<Ranking> GetGlobalRankingAsync(int count, string userId)
    {
        // first day of this month at UTC midnight
        var today = DateTime.UtcNow;
        var monthStart = new DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var sql = """
                  SELECT 
                      u."Username", 
                      up."ProfileSkin", 
                      COUNT(*)        AS "Score"
                  FROM "Users" u
                  JOIN "UserProfiles" up
                    ON up."Id" = u."Id"
                  CROSS JOIN LATERAL UNNEST(up."History") AS his("Id")
                  JOIN "UserHistoryEntries" uhe
                    ON uhe."Id" = his."Id"
                  WHERE uhe."Date" >= {0}
                  GROUP BY u."Username", up."ProfileSkin"
                  ORDER BY "Score" DESC
                  LIMIT {1};
                  """;

        var rankingEntries = await _db.Database
            .SqlQueryRaw<RankingEntry>(sql, monthStart, count)
            .ToListAsync();

        return new Ranking
        {
            RankingEntries = rankingEntries,
            FinishDate = GetRankingFinish(),
            YourPosition = await GetUsersPositionGlobal(userId)
        };
    }

    public async Task<Ranking> GetFriendsRankingAsync(string userId, int count)
    {
        var today = DateTime.UtcNow;
        var monthStart = new DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var sql = """
                    WITH friends AS (
                        SELECT CASE WHEN f."UserId1" = {0} 
                            THEN f."UserId2" 
                            ELSE f."UserId1" 
                        END AS "Id"
                    FROM "Friendships" f
                    WHERE f."UserId1" = {0} OR f."UserId2" = {0}
                    UNION ALL
                    SELECT {0}
                    )
                  SELECT
                    u."Username", up."ProfileSkin", COUNT(uhe."Id") AS "Score"
                  FROM friends as fr
                  JOIN "Users" u ON u."Id" = fr."Id"
                  JOIN "UserProfiles" up ON up."Id" = u."Id"
                  LEFT JOIN LATERAL UNNEST(up."History") AS his("Id") ON true
                  LEFT JOIN "UserHistoryEntries" uhe ON uhe."Id" = his."Id" AND uhe."Date" >= {2}
                  GROUP BY u."Username", up."ProfileSkin"
                  ORDER BY "Score" DESC
                  LIMIT {1}
                  """;

        var rankingEntries = await _db.Database
            .SqlQueryRaw<RankingEntry>(sql, userId, count, monthStart)
            .ToListAsync();

        return new Ranking
        {
            RankingEntries = rankingEntries,
            FinishDate = GetRankingFinish(),
            YourPosition = await GetUsersPositionLocal(userId)
        };
    }


    private DateTime GetRankingFinish()
    {
        var today = DateTime.UtcNow;
        var rankingEnd = new DateTime(today.Year, today.Month, 1).ToUniversalTime().AddMonths(1);
        return rankingEnd;
    }

    private async Task<int> GetUsersPositionGlobal(string userId)
    {
        var today = DateTime.UtcNow;
        var monthStart = new DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var sql = """
                             SELECT 
                              sub."Value"
                             FROM (
                              SELECT u."Id" AS "UserId", ROW_NUMBER() OVER (ORDER BY COUNT(*) DESC) AS "Value"
                              FROM "Users" u
                              JOIN "UserProfiles" up
                                  ON up."Id" = u."Id"
                              CROSS JOIN LATERAL UNNEST(up."History") AS his("Id")
                              JOIN
                             "UserHistoryEntries" uhe
                                  ON uhe."Id" = his."Id"
                              WHERE uhe."Date" >= {1}
                              GROUP BY u."Id" 
                              ) AS sub
                              WHERE sub."UserId" = {0}
                  """;
        var position = await _db.Database.SqlQueryRaw<int>(sql, userId, monthStart).FirstOrDefaultAsync();
        return position == 0 ? _db.Users.Count() : position;
    }

    private async Task<int> GetUsersPositionLocal(string userId)
    {
        var today = DateTime.UtcNow;
        var monthStart = new DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var sql = """
                  SELECT 
                      sub."Value"
                  FROM (
                      SELECT u2."Id" AS "UserId", ROW_NUMBER() OVER (ORDER BY COUNT(*) DESC) AS "Value"
                      FROM "Users" u
                      JOIN "Friendships" f
                          ON f."UserId1" = {0}
                          OR f."UserId2" = {0}
                      JOIN "Users" u2
                          ON (u2."Id" = f."UserId1" OR u2."Id" = f."UserId2")
                      JOIN "UserProfiles" up
                          ON up."Id" = u2."Id" OR up."Id" = {0}
                      CROSS JOIN LATERAL UNNEST(up."History") AS his("Id")
                      JOIN "UserHistoryEntries" uhe
                        ON uhe."Id" = his."Id"
                        WHERE uhe."Date" >= {1}
                        GROUP BY u2."Id"
                  ) AS sub
                  WHERE sub."UserId" = {0}
                  """;
        var position = await _db.Database.SqlQueryRaw<int>(sql, userId, monthStart).FirstOrDefaultAsync();
        return position == 0 ? _db.Friendships.Count(f => f.UserId1 == userId || f.UserId2 == userId) : position;
    }
}
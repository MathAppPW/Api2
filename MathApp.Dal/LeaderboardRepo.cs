using MathApp.Dal.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal;

public class LeaderboardRepo : BaseRepo<Models.Leaderboard>, ILeaderboardRepo
{
    public LeaderboardRepo(MathAppDbContext dbContext) : base(dbContext)
    {
    }
}

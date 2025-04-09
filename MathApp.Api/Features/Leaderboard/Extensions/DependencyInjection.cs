using MathAppApi.Features.UserProgress.Services.Interfaces;
using MathAppApi.Features.UserProgress.Services;
using MathAppApi.Features.Leaderboard.Services.Interfaces;
using MathAppApi.Features.Leaderboard.Services;

namespace MathAppApi.Features.Leaderboard.Extensions;

public static class DependencyInjection
{
    public static void AddLeaderboardServices(this IServiceCollection services)
    {
        services.AddScoped<ILeaderboardService, LeaderboardService>();
    }
}

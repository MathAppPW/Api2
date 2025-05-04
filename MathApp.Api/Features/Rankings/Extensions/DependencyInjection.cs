using MathAppApi.Features.Rankings.Services;

namespace MathAppApi.Features.Rankings.Extensions;

public static class DependencyInjection
{
    public static void AddRankings(this IServiceCollection services)
    {
        services.AddScoped<IRankingService, RankingService>();
    }
}
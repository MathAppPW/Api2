using MathAppApi.Shared.Utils.Interfaces;
using MathAppApi.Shared.Utils;
using MathAppApi.Features.UserProfile.Services.Interfaces;
using MathAppApi.Features.UserProfile.Services;

namespace MathAppApi.Features.UserProfile.Extensions;

public static class DependencyInjection
{
    public static void AddProfileServices(this IServiceCollection services)
    {
        services.AddScoped<ILivesService, LivesService>();
        services.AddScoped<IAchievementsService, AchievementsService>();
    }
}

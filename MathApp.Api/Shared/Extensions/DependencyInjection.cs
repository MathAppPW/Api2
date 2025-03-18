using Dal;
using MathApp.Dal.Interfaces;
using MathAppApi.Shared.Utils;
using MathAppApi.Shared.Utils.Interfaces;

namespace MathAppApi.Shared.Extensions;

public static class DependencyInjection
{
    public static void AddReposShared(this IServiceCollection services)
    {
        services.AddScoped<IHistoryUtils, HistoryUtils>();
    }
}

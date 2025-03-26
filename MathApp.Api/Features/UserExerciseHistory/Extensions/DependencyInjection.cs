using MathAppApi.Features.Authentication.Services.Interfaces;
using MathAppApi.Features.Authentication.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MathAppApi.Features.Progress.Services.Interfaces;
using MathAppApi.Features.Progress.Services;

namespace MathAppApi.Features.UserExerciseHistory.Extensions;

public static class DependencyInjection
{
    public static void AddHistoryServices(this IServiceCollection services)
    {
        services.AddScoped<IProgressService, ProgressService>();
    }
}

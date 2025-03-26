using MathAppApi.Features.Authentication.Services.Interfaces;
using MathAppApi.Features.Authentication.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MathAppApi.Features.UserProgress.Services;
using MathAppApi.Features.UserProgress.Services.Interfaces;

namespace MathAppApi.Features.UserProgress.Extensions;

public static class DependencyInjection
{
    public static void AddProgressServices(this IServiceCollection services)
    {
        services.AddScoped<IProgressService, ProgressService>();
    }
}

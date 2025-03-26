using MathAppApi.Features.Authentication.Services.Interfaces;
using MathAppApi.Features.Authentication.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MathAppApi.Features.Exercise.Services;
using MathAppApi.Features.Exercise.Services.Interfaces;

namespace MathAppApi.Features.Exercise.Extensions;

public static class DependencyInjection
{
    public static void AddExerciseServices(this IServiceCollection services)
    {
        services.AddScoped<IExerciseService, ExerciseService>();
    }
}

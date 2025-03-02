using MathAppApi.Features.Authentication.Services;
using MathAppApi.Features.Authentication.Services.Interfaces;

namespace MathAppApi.Features.Authentication.Extensions;

public static class DependencyInjection
{
    public static void AddAuthenticationServices(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserDataValidator, UserDataValidator>();
    }
}
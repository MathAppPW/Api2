using MathAppApi.Features.Authentication.Services.Interfaces;

namespace MathAppApi.Shared.Cookies.Extensions;

public static class DependencyInjection
{
    public static void AddCookieService(this IServiceCollection services)
    {
        services.AddScoped<ICookieService, CookieService>();
    }

    public static void ConfigureCookies(this WebApplicationBuilder builder)
    {
        var section = builder.Configuration.GetSection("CookieSettings");
        builder.Services.Configure<CookieSettings>(section);
    }
}
using MathAppApi.Shared.Emails.Interfaces;

namespace MathAppApi.Shared.Emails;

public static class DependencyInjection
{
    public static void UseFakeEmailService(this IServiceCollection services)
    {
        services.AddScoped<IEmailService, FakeEmailService>();
    }
}
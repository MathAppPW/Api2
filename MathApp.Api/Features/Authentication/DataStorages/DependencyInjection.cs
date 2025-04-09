namespace MathAppApi.Features.Authentication.DataStorages;

public static class DependencyInjection
{
    public static void AddPasswordResetDataStorage(this IServiceCollection services)
    {
        services.AddSingleton<IPasswordResetDataStorage, PasswordResetDataStorage>();
        services.AddSingleton<IEmailChangeDataStorage, EmailChangeDataStorage>();
    }
}
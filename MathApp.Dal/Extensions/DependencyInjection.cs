using MathApp.Dal.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Dal.Extensions;

public static class DependencyInjection
{
    private const string SqLiteConnectionString = "Data Source=MathApp.db;Cache=Shared";
    
    public static void AddRepos(this IServiceCollection services)
    {
        services.AddScoped<IUserRepo, UserRepo>();
    }

    public static void AddSqLiteDb(this IServiceCollection services, string connectionString = SqLiteConnectionString)
    {
        services.AddDbContext<MathAppDbContext>(o =>
        {
            o.UseSqlite(connectionString);
        });
    }

    public static void AddInMemorySqLiteDb(this IServiceCollection services)
    {
        services.AddDbContext<MathAppDbContext>(o =>
        {
            o.UseSqlite("Data Source=:memory:");
        });
    }
}
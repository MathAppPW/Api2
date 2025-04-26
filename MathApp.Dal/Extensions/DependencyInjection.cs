using MathApp.Dal.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models;

namespace Dal.Extensions;

public static class DependencyInjection
{
    private const string SqLiteConnectionString = "Data Source=MathApp.db;Cache=Shared";
    
    public static void AddRepos(this IServiceCollection services)
    {
        services.AddScoped<IUserRepo, UserRepo>();
        services.AddScoped<IUserProfileRepo, UserProfileRepo>();
        services.AddScoped<IUserHistoryEntryRepo, UserHistoryEntryRepo>();
        services.AddScoped<IFriendshipRepo, FriendshipRepo>();
        services.AddScoped<IFriendRequestRepo, FriendRequestRepo>();
        services.AddScoped<IChapterRepo, ChapterRepo>();
        services.AddScoped<ISubjectRepo, SubjectRepo>();
        services.AddScoped<ILessonRepo, LessonRepo>();
        services.AddScoped<IExerciseRepo, ExerciseRepo>();
        services.AddScoped<ISeriesRepo, SeriesRepo>();
        services.AddScoped<ILeaderboardRepo, LeaderboardRepo>();
        services.AddScoped<ITheoryRepo, TheoryRepo>();
    }

    public static void AddDbFromEnvironment(this IServiceCollection services, string? dbEnvironment,
        string? connectionString)
    {
        switch (dbEnvironment)
        {
            case "sqlite":
                AddSqLiteDb(services, connectionString);
                break;
            case "postgresql":
                if (connectionString is null)
                    throw new ArgumentException(
                        "You must provide connection string for postgresql db! Use CONNECTION_STRING environmental variable.");
                AddPostgresDb(services, connectionString);
                break;
            case null:
                AddSqLiteDb(services, connectionString);
                break;
            default:
                throw new AggregateException("Invalid db environment, check ENVIRONMENT_DB variable");
        }
    }
    
    public static void AddSqLiteDb(this IServiceCollection services, string? connectionString = SqLiteConnectionString)
    {
        connectionString ??= SqLiteConnectionString;
        services.AddDbContext<MathAppDbContext>(o =>
        {
            o.UseSqlite(connectionString);
        });
    }

    public static void AddPostgresDb(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<MathAppDbContext>(o =>
        {
            o.UseNpgsql(connectionString);
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
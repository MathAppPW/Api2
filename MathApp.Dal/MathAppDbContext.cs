using Microsoft.EntityFrameworkCore;
using Models;

namespace Dal;

public class MathAppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Chapter> Chapters { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<Series> Series { get; set; }
    public DbSet<Exercise> Exercises { get; set; }
    public DbSet<UserHistoryEntry> UserHistoryEntries { get; set; }

    public MathAppDbContext(DbContextOptions<MathAppDbContext> options) : base(options)
    {
        if (Database.IsSqlite())
            Database.EnsureCreated();
    }
}
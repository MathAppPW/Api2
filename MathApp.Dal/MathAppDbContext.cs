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
    public DbSet<Friendship> Friendships { get; set; }
    public DbSet<FriendRequest> FriendRequests { get; set; }
    public DbSet<Leaderboard> Leaderboards { get; set; }

    public MathAppDbContext(DbContextOptions<MathAppDbContext> options) : base(options)
    {
        if (Database.IsSqlite())
            Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Leaderboard>()
        .OwnsMany(l => l.Entries, entry =>
        {
            entry.WithOwner();
            entry.Property(e => e.Id)
            .ValueGeneratedOnAdd();
            entry.HasKey(e => e.Id);
        });
    }
}
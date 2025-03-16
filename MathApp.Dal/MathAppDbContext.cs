using Microsoft.EntityFrameworkCore;
using Models;

namespace Dal;

public class MathAppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<UserHistoryEntry> UserHistoryEntries { get; set; }

    public MathAppDbContext(DbContextOptions<MathAppDbContext> options) : base(options)
    {
        if (Database.IsSqlite())
            Database.EnsureCreated();
    }
}
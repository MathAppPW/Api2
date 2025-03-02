using Microsoft.EntityFrameworkCore;
using Models;

namespace Dal;

public class MathAppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public MathAppDbContext(DbContextOptions<MathAppDbContext> options) : base(options)
    {
        if (Database.IsSqlite())
            Database.EnsureCreated();
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Dal;

//this class is used for ef core migrations
public class MathAppDbContextFactory : IDesignTimeDbContextFactory<MathAppDbContext>
{
    public MathAppDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        var optionsBuilder = new DbContextOptionsBuilder<MathAppDbContext>();
        optionsBuilder.UseNpgsql(connectionString);
        return new MathAppDbContext(optionsBuilder.Options);
    }
}
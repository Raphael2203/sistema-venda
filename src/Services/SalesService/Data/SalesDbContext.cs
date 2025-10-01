using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SalesService.Data;

public class SalesDbContext : DbContext
{
    public SalesDbContext(DbContextOptions<SalesDbContext> options) : base(options) { }

    public DbSet<Order> Orders { get; set; } = null!;
}
public class SalesDbContextFactory : IDesignTimeDbContextFactory<SalesDbContext>
{
    public SalesDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("Mysql");

        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("Mysql não está definida.");

        var optionsBuilder = new DbContextOptionsBuilder<SalesDbContext>();
        optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 26)));

        return new SalesDbContext(optionsBuilder.Options);
    }
}
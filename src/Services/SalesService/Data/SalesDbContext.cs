using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SalesService.Models;
using Microsoft.Extensions.Configuration;

public class SalesDbContext : DbContext
{
    public SalesDbContext(DbContextOptions<SalesDbContext> options) : base(options) { }
    public DbSet<Sale> Sales { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Sale>()
            .Property(s => s.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<SaleItem>()
            .Property(si => si.Id)
            .ValueGeneratedOnAdd();

        base.OnModelCreating(modelBuilder);
    }
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
using Microsoft.EntityFrameworkCore;

namespace SalesService.Data
{
    public class SalesDbContext : DbContext
    {
        public SalesDbContext(DbContextOptions<SalesDbContext> options) : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }
    }
}
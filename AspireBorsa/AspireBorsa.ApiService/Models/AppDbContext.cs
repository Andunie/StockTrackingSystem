using Microsoft.EntityFrameworkCore;

namespace AspireBorsa.ApiService.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Stock> Stocks { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using System;

namespace AspireBorsa.Web.Model
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { } 

        public DbSet<ViewStock> Stocks { get; set; }
    }
}

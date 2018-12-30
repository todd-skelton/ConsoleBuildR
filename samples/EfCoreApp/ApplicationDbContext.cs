using EfCoreApp.Models;
using Microsoft.EntityFrameworkCore;

namespace EfCoreApp
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(e =>
            {
                e.HasKey(p => p.Username);
            });
        }
    }
}

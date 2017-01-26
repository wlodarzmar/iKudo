using iKudo.Domain.Model;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace iKudo.Domain.Model
{
    public class KudoDbContext : DbContext
    {
        public KudoDbContext(DbContextOptions<KudoDbContext> options) : base(options)
        {
            Database.SetCommandTimeout(10000);
        }

        public KudoDbContext()
        {
            Database.SetCommandTimeout(10000);
        }

        public DbSet<Company> Companies { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
        }
    }
}

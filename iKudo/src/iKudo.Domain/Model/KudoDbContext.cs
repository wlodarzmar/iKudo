using Microsoft.EntityFrameworkCore;

namespace iKudo.Domain.Model
{
    public class KudoDbContext : DbContext
    {
        public KudoDbContext(DbContextOptions<KudoDbContext> options) : base(options)
        {
        }

        public KudoDbContext()
        {
        }

        public virtual DbSet<Company> Companies { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>().HasKey(x => x.Id);
            modelBuilder.Entity<Company>().Property(x => x.Name).IsRequired();
            modelBuilder.Entity<Company>().Property(x => x.CreatorId).IsRequired();

            base.OnModelCreating(modelBuilder);
        }
    }
}

using Microsoft.EntityFrameworkCore;

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
            Database?.SetCommandTimeout(10000);
        }

        public virtual DbSet<Group> Groups { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Group>().HasKey(x => x.Id);
            modelBuilder.Entity<Group>().Property(x => x.Name).IsRequired();
            modelBuilder.Entity<Group>().Property(x => x.CreatorId).IsRequired();
            modelBuilder.Entity<Group>().Property(x => x.CreationDate).IsRequired();
            modelBuilder.Entity<Group>().Property(x => x.ModificationDate).IsRequired(false);
        }
    }
}

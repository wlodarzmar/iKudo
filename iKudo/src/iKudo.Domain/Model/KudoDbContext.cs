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

        public virtual DbSet<Board> Boards { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Board>().HasKey(x => x.Id);
            modelBuilder.Entity<Board>().Property(x => x.Name).IsRequired();
            modelBuilder.Entity<Board>().Property(x => x.CreatorId).IsRequired();
            modelBuilder.Entity<Board>().Property(x => x.CreationDate).IsRequired();
            modelBuilder.Entity<Board>().Property(x => x.ModificationDate).IsRequired(false);
        }
    }
}

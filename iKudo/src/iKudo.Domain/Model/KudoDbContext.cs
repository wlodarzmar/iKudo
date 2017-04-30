using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

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

        public virtual DbSet<JoinRequest> JoinRequests { get; set; }

        public virtual DbSet<UserBoard> UserBoards { get; set; }

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
            modelBuilder.Entity<Board>().HasMany(x => x.UserBoards).WithOne().HasForeignKey(x => x.BoardId);

            modelBuilder.Entity<JoinRequest>().HasKey(x => x.Id);
            modelBuilder.Entity<JoinRequest>().HasOne(x => x.Board)
                                              .WithMany(x => x.JoinRequests)
                                              .HasForeignKey(x => x.BoardId)
                                              .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserBoard>().HasKey(x => new { x.UserId, x.BoardId });
        }
    }
}

using iKudo.Domain.Logic;
using Microsoft.EntityFrameworkCore;

namespace iKudo.Domain.Model
{
    public class KudoDbContext : DbContext
    {
        public KudoDbContext(DbContextOptions<KudoDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Board> Boards { get; set; }

        public virtual DbSet<JoinRequest> JoinRequests { get; set; }

        public virtual DbSet<UserBoard> UserBoards { get; set; }

        public virtual DbSet<Notification> Notifications { get; set; }

        public virtual DbSet<Kudo> Kudos { get; set; }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<BoardInvitation> BoardInvitations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Ignore<JoinState>();
            modelBuilder.ApplyConfiguration(new KudoConfiguration());
            modelBuilder.ApplyConfiguration(new BoardConfiguration());
            modelBuilder.ApplyConfiguration(new JoinRequestConfiguration());
            modelBuilder.ApplyConfiguration(new UserBoardConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new BoardInvitationConfiguration());
        }
    }
}

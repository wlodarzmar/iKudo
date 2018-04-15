using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iKudo.Domain.Model
{
    public class KudoConfiguration : IEntityTypeConfiguration<Kudo>
    {
        public void Configure(EntityTypeBuilder<Kudo> modelBuilder)
        {
            modelBuilder.HasKey(x => x.Id);
            modelBuilder.Property(x => x.SenderId).IsRequired();
            modelBuilder.HasOne(x => x.Receiver).WithMany().HasForeignKey(x => x.ReceiverId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Property(x => x.ReceiverId).IsRequired();
            modelBuilder.HasOne(x => x.Sender).WithMany().HasForeignKey(x => x.SenderId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.HasOne(x => x.Board)
                                       .WithMany(x => x.Kudos)
                                       .HasForeignKey(x => x.BoardId)
                                       .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Ignore(x => x.ImageExtension);
        }
    }
}

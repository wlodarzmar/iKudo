using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iKudo.Domain.Model
{
    public class KudoConfiguration : IEntityTypeConfiguration<Kudo>
    {
        public void Configure(EntityTypeBuilder<Kudo> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.SenderId).IsRequired();
            builder.HasOne(x => x.Receiver).WithMany().HasForeignKey(x => x.ReceiverId).OnDelete(DeleteBehavior.Restrict);
            builder.Property(x => x.ReceiverId).IsRequired();
            builder.HasOne(x => x.Sender).WithMany().HasForeignKey(x => x.SenderId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Board)
                                       .WithMany(x => x.Kudos)
                                       .HasForeignKey(x => x.BoardId)
                                       .OnDelete(DeleteBehavior.Cascade);
            builder.Ignore(x => x.ImageExtension);
            builder.Ignore(x => x.IsApprovalEnabled);
        }
    }
}

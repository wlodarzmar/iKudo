using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iKudo.Domain.Model
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ReceiverId).IsRequired();
            builder.Property(x => x.SenderId).IsRequired();
            builder.HasOne(x => x.Board)
                   .WithMany()
                   .HasForeignKey(x => x.BoardId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

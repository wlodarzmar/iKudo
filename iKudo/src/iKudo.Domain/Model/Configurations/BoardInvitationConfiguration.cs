using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iKudo.Domain.Model
{
    public class BoardInvitationConfiguration : IEntityTypeConfiguration<BoardInvitation>
    {
        public void Configure(EntityTypeBuilder<BoardInvitation> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Board).WithMany().HasForeignKey(x => x.BoardId);
            builder.HasOne(x => x.Creator).WithMany().HasForeignKey(x => x.CreatorId);
            builder.Property(x => x.Email).IsRequired();
            builder.Property(x => x.CreatorId).IsRequired();
            builder.Property(x => x.CreationDate).IsRequired();
            builder.Property(x => x.Code).IsRequired();
            builder.Property(x => x.BoardId).IsRequired();
        }
    }
}

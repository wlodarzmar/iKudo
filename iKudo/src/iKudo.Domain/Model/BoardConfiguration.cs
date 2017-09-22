using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iKudo.Domain.Model
{
    public class BoardConfiguration : IEntityTypeConfiguration<Board>
    {
        public void Configure(EntityTypeBuilder<Board> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.CreatorId).IsRequired();
            builder.Property(x => x.CreationDate).IsRequired();
            builder.Property(x => x.ModificationDate).IsRequired(false);
            builder.HasMany(x => x.UserBoards).WithOne().HasForeignKey(x => x.BoardId);
        }
    }
}

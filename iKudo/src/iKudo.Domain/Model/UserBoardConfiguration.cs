using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iKudo.Domain.Model
{
    public class UserBoardConfiguration : IEntityTypeConfiguration<UserBoard>
    {
        public void Configure(EntityTypeBuilder<UserBoard> builder)
        {
            builder.HasKey(x => new { x.UserId, x.BoardId });
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iKudo.Domain.Model
{
    public class JoinRequestConfiguration : IEntityTypeConfiguration<JoinRequest>
    {
        public void Configure(EntityTypeBuilder<JoinRequest> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Board)
                        .WithMany(x => x.JoinRequests)
                        .HasForeignKey(x => x.BoardId)
                        .OnDelete(DeleteBehavior.Cascade);
            builder.Property(x => x.CandidateId).IsRequired();
        }
    }
}

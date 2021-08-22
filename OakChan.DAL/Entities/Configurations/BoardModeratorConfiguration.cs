using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OakChan.Identity;

namespace OakChan.DAL.Entities.Configurations
{
    public class BoardModeratorConfiguration : IEntityTypeConfiguration<BoardModerator>
    {
        public void Configure(EntityTypeBuilder<BoardModerator> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.UserId, x.BoardKey })
                .IsUnique();

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Board>()
                .WithMany()
                .HasForeignKey(x => x.BoardKey)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Vookaba.DAL.Entities.Configurations
{
    public class BanConfiguration : IEntityTypeConfiguration<Ban>
    {
        public void Configure(EntityTypeBuilder<Ban> builder)
        {
            builder.HasKey(b => b.Id);

            builder.HasOne(b => b.Creator)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .IsRequired(true);

            builder.HasOne(b => b.Board)
                .WithMany()
                .HasForeignKey(x => x.BoardKey)
                .IsRequired(false);

            builder.HasOne<Post>()
                .WithMany()
                .HasForeignKey(b => b.PostId);

            builder.HasOne(b => b.BannedAuthor)
                .WithMany()
                .HasForeignKey(b => b.BannedAothorToken)
                .IsRequired(false);

            builder.Property(b => b.BannedNetwork)
                .IsRequired(false);

            builder.Property(b => b.Reason)
                .HasMaxLength(500)
                .IsRequired(true);

            builder.HasIndex(x => new { x.IsCanceled, x.Expired, x.BoardKey, x.BannedNetwork, x.BannedAothorToken });
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OakChan.DAL.Entities.Configurations
{
    public class BoardConfiguration : IEntityTypeConfiguration<Board>
    {
        public void Configure(EntityTypeBuilder<Board> builder)
        {
            builder.HasKey(b => b.Key);

            builder.Property(b => b.Name)
                .HasMaxLength(128)
                .IsRequired();

            builder.HasMany(b => b.Threads)
                .WithOne(t => t.Board)
                .HasForeignKey(t => t.BoardId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(b => b.IsHidden)
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(b => b.IsDisabled)
                .HasDefaultValue(false)
                .IsRequired();
        }
    }
}

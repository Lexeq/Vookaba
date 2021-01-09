using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OakChan.DAL.Entities;

namespace OakChan.Models.DB.Configurations
{
    public class ThreadConfiguation
    {
        public void Configure(EntityTypeBuilder<Thread> builder)
        {
            builder.HasKey(t => t.Id);

            builder.HasOne(t => t.Board)
                .WithMany(b => b.Threads)
                .HasForeignKey(t => t.BoardId)
                .IsRequired();

            builder.HasMany(t => t.Posts)
                .WithOne()
                .HasForeignKey(a => a.ThreadId);

            builder.Property(p => p.Subject)
                .HasMaxLength(512)
                .IsRequired(false);
        }
    }
}

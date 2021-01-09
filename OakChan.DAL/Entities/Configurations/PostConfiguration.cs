using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OakChan.DAL.Entities.Configurations
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .IsRequired(true)
                .HasDefaultValue("Аноним");

            builder.HasOne(j => j.Thread)
                .WithMany(t => t.Posts)
                .HasForeignKey(t => t.ThreadId)
                .IsRequired();

            builder.Property(p => p.CreationTime)
                .IsRequired();

            builder.Property(p => p.Message)
                .IsRequired(false)
                .HasMaxLength(4096);

            builder.HasOne<Anonymous>()
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .IsRequired();

            builder.HasOne(p => p.Image)
                .WithMany();
        }
    }
}

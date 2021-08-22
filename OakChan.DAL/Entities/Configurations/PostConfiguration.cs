using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using OakChan.Common;

namespace OakChan.DAL.Entities.Configurations
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .IsRequired(false);

            builder.Property(p => p.Created)
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.Property(p => p.AuthorIP)
                .IsRequired(true);

            builder.Property(p => p.AuthorUserAgent)
                .IsRequired(true);

            builder.Property(p => p.Message)
                .IsRequired(false)
                .HasMaxLength(OakConstants.PostConstants.MessageMaxLength);

            builder.HasOne<AnonymousToken>()
                .WithMany()
                .HasForeignKey(p => p.AnonymousToken)
                .IsRequired();

            builder.HasOne(p => p.Thread)
                .WithMany(t => t.Posts)
                .HasForeignKey(p => p.ThreadId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(p => p.Number)
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.HasIndex(p => new { p.ThreadId, p.Number })
                .HasSortOrder(SortOrder.Ascending, SortOrder.Ascending);

            builder.HasIndex(p => new { p.IsOP, p.ThreadId })
                .HasSortOrder(SortOrder.Descending, SortOrder.Ascending);
        }
    }
}

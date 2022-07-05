using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Vookaba.Common;
using Vookaba.DAL.Entities;
using System;

namespace Vookaba.Models.DB.Configurations
{
    public class ThreadConfiguation : IEntityTypeConfiguration<Thread>
    {
        public void Configure(EntityTypeBuilder<Thread> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Subject)
                .HasMaxLength(ApplicationConstants.ThreadConstants.SubjectMaxLength)
                .IsRequired(false);

            builder.Property(t => t.LastBump)
                .HasDefaultValue(DateTime.MinValue)
                .ValueGeneratedOnUpdate();

            builder.Property(t => t.LastHit)
                .HasDefaultValue(DateTime.MinValue)
                .ValueGeneratedOnUpdate();

            builder.Property(t => t.PostsCount)
                .HasDefaultValue(0)
                .ValueGeneratedOnAddOrUpdate();

            builder.Property(t => t.PostsWithAttachmentnsCount)
                .HasDefaultValue(0)
                .ValueGeneratedOnAddOrUpdate();

            builder.Property(t => t.Created)
                .IsRequired(true);

            builder.HasOne(t => t.Board)
                .WithMany(b => b.Threads)
                .HasForeignKey(t => t.BoardKey)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasIndex(t => new { t.BoardKey, t.IsPinned, t.LastBump })
                .HasSortOrder(SortOrder.Ascending, SortOrder.Descending, SortOrder.Descending);

            builder.HasIndex(t => new { t.BoardKey, t.IsPinned, t.Created })
                .HasSortOrder(SortOrder.Ascending, SortOrder.Descending, SortOrder.Descending);
        }
    }
}

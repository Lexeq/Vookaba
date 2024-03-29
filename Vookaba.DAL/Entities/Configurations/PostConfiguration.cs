﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Vookaba.Common;
using Vookaba.Identity;

namespace Vookaba.DAL.Entities.Configurations
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .IsRequired(false);

            builder.Property(p => p.Created)
                .IsRequired();

            builder.Property(p => p.IP)
                .HasColumnName("AuthorIP")
                .IsRequired(true);

            builder.Property(p => p.UserAgent)
                .HasColumnName("AuthorUserAgent")
                .IsRequired(true);

            builder.Property(p => p.PlainMessageText)
                .IsRequired(false)
                .HasMaxLength(ApplicationConstants.PostConstants.MessageMaxLength);

            builder.Property(p => p.HtmlEncodedMessage)
                .HasColumnName("Message");

            builder.HasOne<AuthorToken>()
                .WithMany()
                .HasForeignKey(p => p.AuthorToken)
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

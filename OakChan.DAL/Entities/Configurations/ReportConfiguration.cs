﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OakChan.DAL.Entities.Configurations
{
    public class ReportConfiguration : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Created)
                .IsRequired();

            builder.Property(r => r.IP)
                .HasColumnName("ComplainantIP")
                .IsRequired();

            builder.Property(r => r.UserAgent)
                .HasColumnName("ComplainantUserAgent")
                .IsRequired();

            builder.HasOne<AuthorToken>()
                .WithMany()
                .HasForeignKey(r => r.AuthorToken)
                .IsRequired();

            builder.HasIndex(r => r.IsProcessed);

            builder.Property(r => r.IsProcessed)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasOne(r => r.ProcessedBy)
                .WithMany();

            builder.Property(r => r.Reason)
                .HasMaxLength(512)
                .IsRequired(true);

            builder.HasOne(p => p.Post)
                .WithMany()
                .HasForeignKey(r => r.PostId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(r => new { r.IsProcessed, r.Created });
        }
    }
}

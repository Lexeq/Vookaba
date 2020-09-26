using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OakChan.Models.DB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OakChan.Models.DB.Configurations
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .IsRequired(true)
                .HasDefaultValue("Аноним");

            builder.HasOne<Thread>()
                .WithMany(t => t.Posts)
                .HasForeignKey(t => t.ThreadId)
                .IsRequired();

            builder.Property(p => p.CreationTime)
                .IsRequired();

            builder.Property(p => p.Message)
                .IsRequired()
                .HasMaxLength(4096);

            builder.HasOne<Anonymous>()
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .IsRequired();

            builder.Property(p => p.Subject)
                .IsRequired(false);

            builder.HasOne(p => p.Image)
                .WithMany();
        }
    }
}

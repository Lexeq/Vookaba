using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OakChan.Models.DB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OakChan.Models.DB.Configurations
{
    public class ThreadConfiguation
    {
        public void Configure(EntityTypeBuilder<Thread> builder)
        {
            builder.HasKey(t => t.Id);

            builder.HasOne<Board>()
                .WithMany(b=>b.Threads)
                .IsRequired()
                .HasForeignKey(t => t.BoardId);

            builder.HasMany(t => t.Posts)
                .WithOne()
                .HasForeignKey(a => a.ThreadId);
        }
    }
}

﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OakChan.Models.DB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OakChan.Models.DB.Configurations
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
                .WithOne()
                .HasForeignKey(t => t.BoardId);
        }
    }
}

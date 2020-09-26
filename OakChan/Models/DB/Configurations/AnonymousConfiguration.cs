using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OakChan.Models.DB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OakChan.Models.DB.Configurations
{
    public class AnonymousConfiguration : IEntityTypeConfiguration<Anonymous>
    {
        public void Configure(EntityTypeBuilder<Anonymous> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Created).IsRequired();

            builder.Property(a => a.IP).IsRequired();
        }
    }
}
